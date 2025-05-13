import React, { useEffect, useState } from 'react';
import { ref, onValue } from 'firebase/database';
import { db } from '../FireBaseDB';
import { Box, Typography, Paper, Grid } from '@mui/material';
import { BarChart, Bar, XAxis, YAxis, Tooltip, Legend } from 'recharts';

export default function Dashboard() {
  const [players, setPlayers] = useState([]);

  useEffect(() => {
    const playersRef = ref(db, 'players');
    onValue(playersRef, (snapshot) => {
      const data = snapshot.val();
      if (data) {
        const playerList = Object.keys(data).map((key) => ({
          id: key,
          ...data[key],
        }));
        setPlayers(playerList);
      }
    });
  }, []);

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Mathio Bros - Teacher Dashboard
      </Typography>

      <Grid container spacing={3}>
        {/* Player Stats */}
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Player Progress</Typography>
            <BarChart
              width={500}
              height={300}
              data={players}
              margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
            >
              <XAxis dataKey="id" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="level" fill="#8884d8" name="Level" />
              <Bar dataKey="score" fill="#82ca9d" name="Score" />
            </BarChart>
          </Paper>
        </Grid>

        {/* Player List */}
        <Grid item xs={12} md={6}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6">Players</Typography>
            <ul>
              {players.map((player) => (
                <li key={player.id}>
                  <strong>{player.id}</strong> - Level: {player.level}, Score: {player.score}
                </li>
              ))}
            </ul>
          </Paper>
        </Grid>
      </Grid>
    </Box>
  );
}