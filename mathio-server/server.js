const express = require("express");
const admin = require("firebase-admin");
const cors = require("cors");
const app = express();
const bcrypt = require("bcryptjs");
//const signInWithEmailAndPassword = require("firebase/auth");

// --- Firebase Admin Initialization --- //
const serviceAccount = require("./serviceAccountKey.json"); // Replace with your downloaded file

admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://mathiobros-default-rtdb.firebaseio.com/", // Replace with your DB URL
});

const db = admin.database();

// --- Middleware --- //
app.use(cors());
app.use(express.json());

// --- API Endpoints --- //

// 1. Sign Up
const saltRounds = 10;

app.post("/signup", async (req, res) => {
  const { email, password, username } = req.body;

  try {
    // 1. Check if user already exists
    const existingUserSnap = await db
      .ref("users")
      .orderByChild("email")
      .equalTo(email)
      .once("value");
    if (existingUserSnap.exists()) {
      return res
        .status(400)
        .json({ success: false, message: "Email already exists" });
    }

    // 2. Hash the password
    const hashedPassword = await bcrypt.hash(password, saltRounds);

    // 3. Create a new user entry
    const newUserRef = db.ref("users").push();
    const uid = newUserRef.key;

    await newUserRef.set({
      email,
      password: hashedPassword, // hashed!
      username,
      createdAt: new Date().toISOString(),
      lastLogin: new Date().toISOString(),
    });

    res.status(200).json({ success: true, uid });
  } catch (error) {
    console.error(error);
    res.status(500).json({ success: false, message: "Server error" });
  }
});

//2. login
app.post("/login", async (req, res) => {
  const { email, password } = req.body;

  try {
    // 1. Find user by email
    const snapshot = await db
      .ref("users")
      .orderByChild("email")
      .equalTo(email)
      .once("value");
    if (!snapshot.exists()) {
      return res
        .status(401)
        .json({ success: false, message: "Invalid email or password" });
    }

    const userId = Object.keys(snapshot.val())[0];
    const userData = snapshot.val()[userId];

    // 2. Check password using bcrypt
    const match = await bcrypt.compare(password, userData.password);
    if (!match) {
      return res
        .status(401)
        .json({ success: false, message: "Invalid email or password" });
    }

    // 3. Update last login
    await db.ref(`users/${userId}/lastLogin`).set(new Date().toISOString());

    // 4. Return user info (no token)
    res
      .status(200)
      .json({ success: true, uid: userId, username: userData.username });
  } catch (error) {
    console.error(error);
    res.status(500).json({ success: false, message: "Server error" });
  }
});

// 3. Save Player Data
app.post("/savePlayerData", async (req, res) => {
  const { playerId, level, score, timeSpent } = req.body;

  try {
    const ref = db.ref(`players/${playerId}`);
    await ref.set({
      level,
      score,
      timeSpent,
      lastUpdated: new Date().toISOString(),
    });
    res.status(200).send({ success: true });
  } catch (error) {
    res.status(500).send({ success: false, message: "Failed to save data" });
  }
});

// 4. Get Player Data
app.get("/getPlayerData/:playerId", async (req, res) => {
  const { playerId } = req.params;

  try {
    const snapshot = await db.ref(`players/${playerId}`).once("value");
    const playerData = snapshot.val();
    res.status(200).send(playerData || {});
  } catch (error) {
    res.status(500).send({ success: false, message: "Failed to fetch data" });
  }
});

// 5. Get Level Parameters (optional)
app.get("/getLevelParams/:levelId", async (req, res) => {
  const { levelId } = req.params;

  try {
    const snapshot = await db.ref(`levels/${levelId}`).once("value");
    const levelParams = snapshot.val();
    res.status(200).send(levelParams || {});
  } catch (error) {
    res
      .status(500)
      .send({ success: false, message: "Failed to fetch level data" });
  }
});

// 6. Check if email or username exists
app.get("/checkUser", async (req, res) => {
  const { field, value } = req.query; // field = 'email' or 'username'

  try {
    const snapshot = await db
      .ref("users")
      .orderByChild(field)
      .equalTo(value)
      .once("value");
    const exists = snapshot.exists();
    res.status(200).send({ exists });
  } catch (error) {
    res.status(500).send({ success: false, message: error.message });
  }
});

// --- Start Server --- //
const PORT = 3000;
app.listen(PORT, () => {
  console.log(`🚀 Server running at http://localhost:${PORT}`);
});
