const express = require("express");
const admin = require("firebase-admin");
const cors = require("cors");
const app = express();
const bcrypt = require("bcryptjs");
const { v4: uuidv4 } = require("uuid");
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

// Update Player Data: add score and timeSpent, update level if higher
app.post("/updatePlayerData", async (req, res) => {
  const { playerId, score, timeSpent, level } = req.body;

  if (!playerId || score == null || timeSpent == null || level == null) {
    return res.status(400).send({ success: false, message: "Missing fields" });
  }

  try {
    const ref = db.ref(`players/${playerId}`);
    const snapshot = await ref.once("value");
    const currentData = snapshot.val() || {};

    const updatedScore = (currentData.score || 0) + score;
    const updatedTimeSpent = (currentData.timeSpent || 0) + timeSpent;
    const updatedLevel = level;

    await ref.update({
      score: updatedScore,
      timeSpent: updatedTimeSpent,
      level: updatedLevel,
      lastUpdated: new Date().toISOString(),
    });

    res.status(200).send({
      success: true,
      message: "Player data updated",
      score: updatedScore,
      timeSpent: updatedTimeSpent,
      level: updatedLevel,
    });
  } catch (error) {
    console.error(error);
    res
      .status(500)
      .send({ success: false, message: "Failed to update player data" });
  }
});

// 6. Save Level
app.post("/saveLevel", async (req, res) => {
  const { levelName, levelData } = req.body;

  // Validate input
  if (!levelName || !levelData) {
    return res.status(400).json({
      success: false,
      message: "Missing level name or data",
    });
  }

  try {
    const levelsRef = db.ref("levels");

    // 1. Check if a level with the same name exists
    const existingSnap = await levelsRef
      .orderByChild("levelName")
      .equalTo(levelName)
      .once("value");

    const now = new Date().toISOString();

    if (existingSnap.exists()) {
      // 2. Level exists → update it
      const existingLevelId = Object.keys(existingSnap.val())[0];
      const existingRef = db.ref(`levels/${existingLevelId}`);

      await existingRef.update({
        levelData,
        updatedAt: now,
      });

      return res.status(200).json({
        success: true,
        message: "Level updated",
        levelId: existingLevelId,
        updated: true,
      });
    } else {
      // 3. Level does not exist → create new
      const newLevelRef = levelsRef.push();
      const levelId = newLevelRef.key;

      await newLevelRef.set({
        levelId,
        levelName,
        levelData,
        createdAt: now,
      });

      return res.status(200).json({
        success: true,
        message: "Level created",
        levelId,
        updated: false,
      });
    }
  } catch (error) {
    console.error("Error saving/updating level:", error);
    res.status(500).json({ success: false, message: "Server error" });
  }
});

// 7. Get All Levels (just names and IDs)
app.get("/getLevels", async (req, res) => {
  try {
    const snapshot = await db.ref("levels").once("value");
    const levels = [];

    snapshot.forEach((childSnapshot) => {
      const level = childSnapshot.val();
      levels.push({
        id: level.levelId,
        name: level.levelName,
      });
    });

    res.status(200).json({ success: true, levels });
  } catch (error) {
    res.status(500).json({ success: false, message: "Failed to fetch levels" });
  }
});

// 8. Get Level Data by ID
app.get("/getLevelById/:levelId", async (req, res) => {
  const { levelId } = req.params;

  try {
    const snapshot = await db.ref(`levels/${levelId}`).once("value");
    const levelData = snapshot.val();

    if (!levelData) {
      return res
        .status(404)
        .json({ success: false, message: "Level not found" });
    }

    res.status(200).json({
      success: true,
      level: {
        id: levelData.levelId,
        name: levelData.levelName,
        data: JSON.parse(levelData.levelData),
      },
    });
  } catch (error) {
    res.status(500).json({ success: false, message: "Failed to fetch level" });
  }
});

// 9. Get Level Data by Name
app.get("/getLevelByName/:levelName", async (req, res) => {
  const { levelName } = req.params;

  try {
    // Query for level with the exact name
    const snapshot = await db
      .ref("levels")
      .orderByChild("levelName")
      .equalTo(levelName)
      .once("value");

    if (!snapshot.exists()) {
      return res.status(404).json({
        success: false,
        message: `Level "${levelName}" not found.`,
      });
    }

    const levels = snapshot.val();
    const levelData = Object.values(levels)[0]; // Only one should exist

    console.log(levelData);

    return res.status(200).json({
      success: true,
      level: levelData.levelData,
    });
  } catch (error) {
    console.error("Error fetching level by name:", error);
    return res
      .status(500)
      .json({ success: false, message: "Failed to fetch level" });
  }
});

// --- Start Server --- //
const PORT = 3001;
app.listen(PORT, () => {
  console.log(`🚀 Server running at http://localhost:${PORT}`);
});
