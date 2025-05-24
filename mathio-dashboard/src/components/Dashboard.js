import React, { useState, useEffect } from 'react';
import { ref, onValue, update } from 'firebase/database';
import { db } from '../FireBaseDB';
import { 
  Box, Typography, Paper, Grid, Button, 
  Tabs, Tab, List, ListItem, ListItemText,
  Chip, IconButton, Dialog, DialogTitle, 
  DialogContent, TextField, DialogActions
} from '@mui/material';
import { 
  BarChart, Bar, XAxis, YAxis, Tooltip, Legend, 
  PieChart, Pie, Cell, ResponsiveContainer 
} from 'recharts';
import {
  Person, Class, School, Edit, 
  Add, Delete, Refresh, LockReset
} from '@mui/icons-material';

// Sample class data (replace with Firebase fetch)
const sampleClasses = [
  { id: 'class1', name: 'Math 101', students: 15 },
  { id: 'class2', name: 'Algebra II', students: 12 }
];

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042'];

export default function TeacherDashboard() {
  const [players, setPlayers] = useState([]);
  const [activeTab, setActiveTab] = useState(0);
  const [selectedClass, setSelectedClass] = useState(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [newPlayer, setNewPlayer] = useState({ name: '', email: '' });

  // Fetch player data
  useEffect(() => {
    const playersRef = ref(db, 'players');
    onValue(playersRef, (snapshot) => {
      const data = snapshot.val();
      if (data) {
        setPlayers(Object.keys(data).map(key => ({ id: key, ...data[key] })));
      }
    });
  }, []);

  // Reset password handler
  const handleResetPassword = (playerId) => {
    update(ref(db, `players/${playerId}`), { 
      needsPasswordReset: true 
    });
  };

  // Add new player
  const handleAddPlayer = () => {
    update(ref(db, `players/${Date.now()}`), {
      ...newPlayer,
      level: 1,
      score: 0,
      joinDate: new Date().toISOString()
    });
    setOpenDialog(false);
    setNewPlayer({ name: '', email: '' });
  };

  // Data for charts
  const progressData = players.map(p => ({
    name: p.name || `Player ${p.id.slice(0,4)}`,
    level: p.level,
    score: p.score
  }));

  const classDistribution = [
    { name: 'Math 101', value: 15 },
    { name: 'Algebra II', value: 12 }
  ];

  return (
    <Box sx={{ p: 3 }}>
      {/* Header with actions */}
      <Box sx={{ 
        display: 'flex', 
        justifyContent: 'space-between',
        alignItems: 'center',
        mb: 3
      }}>
        <Typography variant="h4">Mathio Bros Teacher Dashboard</Typography>
        <Button 
          variant="contained" 
          startIcon={<Add />}
          onClick={() => setOpenDialog(true)}
        >
          Add Student
        </Button>
      </Box>

      {/* Main tabs */}
      <Paper sx={{ mb: 3 }}>
        <Tabs 
          value={activeTab} 
          onChange={(e, newVal) => setActiveTab(newVal)}
          variant="fullWidth"
        >
          <Tab label="Overview" icon={<School />} />
          <Tab label="Students" icon={<Person />} />
          <Tab label="Classes" icon={<Class />} />
        </Tabs>
      </Paper>

      {/* Tab content */}
      {activeTab === 0 && (
        <Grid container spacing={3}>
          {/* Progress chart */}
          <Grid item xs={12} md={8}>
            <Paper sx={{ p: 2, height: '100%' }}>
              <Typography variant="h6" gutterBottom>
                Class Progress
              </Typography>
              <ResponsiveContainer width="100%" height={400}>
                <BarChart data={progressData}>
                  <XAxis dataKey="name" />
                  <YAxis />
                  <Tooltip />
                  <Legend />
                  <Bar dataKey="level" fill="#8884d8" name="Level" />
                  <Bar dataKey="score" fill="#82ca9d" name="Score" />
                </BarChart>
              </ResponsiveContainer>
            </Paper>
          </Grid>

          {/* Class distribution */}
          <Grid item xs={12} md={4}>
            <Paper sx={{ p: 2, height: '100%' }}>
              <Typography variant="h6" gutterBottom>
                Class Distribution
              </Typography>
              <ResponsiveContainer width="100%" height={300}>
                <PieChart>
                  <Pie
                    data={classDistribution}
                    cx="50%"
                    cy="50%"
                    outerRadius={80}
                    fill="#8884d8"
                    dataKey="value"
                    label
                  >
                    {classDistribution.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            </Paper>
          </Grid>
        </Grid>
      )}

      {activeTab === 1 && (
        <Paper sx={{ p: 2 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
            <Typography variant="h6">Student Management</Typography>
            <Button 
              size="small" 
              startIcon={<Refresh />}
              onClick={() => window.location.reload()}
            >
              Refresh
            </Button>
          </Box>
          
          <List>
            {players.map(player => (
              <ListItem 
                key={player.id}
                secondaryAction={
                  <>
                    <IconButton onClick={() => handleResetPassword(player.id)}>
                      <LockReset />
                    </IconButton>
                    <IconButton edge="end">
                      <Edit />
                    </IconButton>
                  </>
                }
              >
                <ListItemText
                  primary={player.name || `Player ${player.id.slice(0,4)}`}
                  secondary={
                    <>
                      Level: <Chip label={player.level} size="small" /> | 
                      Score: <Chip label={player.score} size="small" color="success" />
                    </>
                  }
                />
              </ListItem>
            ))}
          </List>
        </Paper>
      )}

      {activeTab === 2 && (
        <Grid container spacing={3}>
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 2 }}>
              <Typography variant="h6" gutterBottom>
                Class Management
              </Typography>
              <List>
                {sampleClasses.map(cls => (
                  <ListItem 
                    key={cls.id}
                    button
                    selected={selectedClass === cls.id}
                    onClick={() => setSelectedClass(cls.id)}
                  >
                    <ListItemText
                      primary={cls.name}
                      secondary={`${cls.students} students`}
                    />
                  </ListItem>
                ))}
              </List>
              <Button fullWidth startIcon={<Add />}>
                Create New Class
              </Button>
            </Paper>
          </Grid>

          {selectedClass && (
            <Grid item xs={12} md={6}>
              <Paper sx={{ p: 2 }}>
                <Typography variant="h6">
                  {sampleClasses.find(c => c.id === selectedClass).name}
                </Typography>
                <Typography color="textSecondary" gutterBottom>
                  Class Statistics
                </Typography>
                <Box sx={{ mt: 2 }}>
                  <Button variant="outlined" sx={{ mr: 1 }}>
                    Assign Levels
                  </Button>
                  <Button variant="outlined" sx={{ mr: 1 }}>
                    View Reports
                  </Button>
                  <Button color="error" startIcon={<Delete />}>
                    Archive Class
                  </Button>
                </Box>
              </Paper>
            </Grid>
          )}
        </Grid>
      )}

      {/* Add player dialog */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
        <DialogTitle>Add New Student</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Student Name"
            fullWidth
            value={newPlayer.name}
            onChange={(e) => setNewPlayer({...newPlayer, name: e.target.value})}
          />
          <TextField
            margin="dense"
            label="Email"
            type="email"
            fullWidth
            value={newPlayer.email}
            onChange={(e) => setNewPlayer({...newPlayer, email: e.target.value})}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button onClick={handleAddPlayer} variant="contained">
            Add
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}