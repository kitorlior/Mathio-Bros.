import { initializeApp } from "firebase/app";
import { getDatabase, ref } from "firebase/database";
import { getAuth } from "firebase/auth";  // Add this import
import { getAnalytics } from "firebase/analytics";

const firebaseConfig = {
  apiKey: "AIzaSyBSctGdRt3Dg8Y-p7hRT_3ncaH_BlvkA_k",
  authDomain: "mathiobros.firebaseapp.com",
  databaseURL: "https://mathiobros-default-rtdb.firebaseio.com",
  projectId: "mathiobros",
  storageBucket: "mathiobros.firebasestorage.app",
  messagingSenderId: "80064857116",
  appId: "1:80064857116:web:a1ad0859ba0211840a4bd9",
  measurementId: "G-N8KYZXT2SB"
};

const app = initializeApp(firebaseConfig);
const db = getDatabase(app);
const auth = getAuth(app);  // Initialize auth
const analytics = getAnalytics(app);

export { db, ref, auth };  // Export auth
