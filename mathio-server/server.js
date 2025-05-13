const express = require('express');
const admin = require('firebase-admin');
const cors = require('cors');

// Initialize Firebase Admin SDK
const serviceAccount = require('./serviceAccountKey.json'); // Download from Firebase
admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: 'https://mathio-bros-default-rtdb.firebaseio.com' // Replace with your DB URL
});

const db = admin.database();
const app = express();
app.use(cors()); // Allow Unity to call this server
app.use(express.json()); // Parse JSON requests

// --- API Endpoints --- //

// 1. Save Player Data
app.post('/savePlayerData', async (req, res) => {
  const { playerId, level, score, timeSpent } = req.body;

  try {
    const ref = db.ref(`players/${playerId}`);
    await ref.set({
      level,
      score,
      timeSpent,
      lastUpdated: new Date().toISOString()
    });
    res.status(200).send({ success: true });
  } catch (error) {
    res.status(500).send({ error: 'Failed to save data' });
  }
});

// 2. Fetch Player Data
app.get('/getPlayerData/:playerId', async (req, res) => {
  const { playerId } = req.params;

  try {
    const snapshot = await db.ref(`players/${playerId}`).once('value');
    const playerData = snapshot.val();
    res.status(200).send(playerData || {});
  } catch (error) {
    res.status(500).send({ error: 'Failed to fetch data' });
  }
});

// 3. Fetch Level Parameters (e.g., difficulty)
app.get('/getLevelParams/:levelId', async (req, res) => {
  const { levelId } = req.params;

  try {
    const snapshot = await db.ref(`levels/${levelId}`).once('value');
    const levelParams = snapshot.val();
    res.status(200).send(levelParams || {});
  } catch (error) {
    res.status(500).send({ error: 'Failed to fetch level data' });
  }
});

const { getAuth, createUserWithEmailAndPassword } = require('firebase/auth');
const { initializeApp } = require('firebase/app');

// Initialize Firebase Auth (Client SDK)
const firebaseConfig = {
  apiKey: "YOUR_API_KEY",
  authDomain: "YOUR_PROJECT_ID.firebaseapp.com",
  databaseURL: "https://YOUR_PROJECT_ID.firebaseio.com",
  projectId: "YOUR_PROJECT_ID",
  storageBucket: "YOUR_PROJECT_ID.appspot.com",
  messagingSenderId: "YOUR_SENDER_ID",
  appId: "YOUR_APP_ID"
};

const firebaseApp = initializeApp(firebaseConfig);
const auth = getAuth(firebaseApp);

// Sign Up
app.post('/signup', async (req, res) => {
  const { email, password, username } = req.body;

  try {
    // 1. Create user in Firebase Auth
    const userCredential = await createUserWithEmailAndPassword(auth, email, password);
    const uid = userCredential.user.uid;

    // 2. Save additional user data to Realtime DB
    await db.ref(`users/${uid}`).set({
      email,
      username,
      createdAt: new Date().toISOString(),
      lastLogin: new Date().toISOString()
    });

    res.status(200).send({ success: true, uid });
  } catch (error) {
    res.status(400).send({ error: error.message });
  }
});

const { signInWithEmailAndPassword } = require('firebase/auth');

// Login
app.post('/login', async (req, res) => {
  const { email, password } = req.body;

  try {
    // 1. Authenticate user
    const userCredential = await signInWithEmailAndPassword(auth, email, password);
    const uid = userCredential.user.uid;

    // 2. Update last login time
    await db.ref(`users/${uid}/lastLogin`).set(new Date().toISOString());

    // 3. Fetch user data
    const snapshot = await db.ref(`users/${uid}`).once('value');
    const userData = snapshot.val();

    res.status(200).send({ success: true, user: userData });
  } catch (error) {
    res.status(400).send({ error: error.message });
  }
});

// Check if username/email exists
app.get('/checkUser', async (req, res) => {
    const { field, value } = req.query; // field = 'email' or 'username'
  
    try {
      const snapshot = await db.ref('users').orderByChild(field).equalTo(value).once('value');
      const exists = snapshot.exists();
      res.status(200).send({ exists });
    } catch (error) {
      res.status(500).send({ error: error.message });
    }
  });

// Start the server
const PORT = 3000;
app.listen(PORT, () => {
  console.log(`Server running on http://localhost:${PORT}`);
});