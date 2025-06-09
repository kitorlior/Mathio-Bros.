import React, { useState, useEffect } from 'react';
import { ref, onValue, update, push, set } from 'firebase/database';
import { db } from '../FireBaseDB';
import { 
  Box, Typography, Paper, Grid, Button, 
  Tabs, Tab, List, ListItem, ListItemText,
  Chip, IconButton, Dialog, DialogTitle, 
  DialogContent, TextField, DialogActions,
  Avatar, LinearProgress, Divider, Badge,
  Card, CardContent, CardHeader, Menu, MenuItem, 
  ListItemAvatar, ListItemIcon
} from '@mui/material';
import {
  BarChart, Bar, XAxis, YAxis, Tooltip, Legend, 
  PieChart, Pie, Cell, ResponsiveContainer, AreaChart, Area,
  RadarChart, PolarGrid, PolarAngleAxis, PolarRadiusAxis, Radar
} from 'recharts';
import {
  Person, Class, School, Edit, 
  Add, Delete, Refresh, LockReset,
  MoreVert, Email, BarChart as BarChartIcon,
  Timeline, Assessment, Group, Star,
  Close
} from '@mui/icons-material';
import { motion } from 'framer-motion';
import { styled } from '@mui/material/styles';

// Color scheme
const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8'];
const GRADIENT_COLORS = ['#3f51b5', '#2196f3'];

// Mock data generators (replace with real data)
const generatePlayerData = (count) => {
  return Array.from({ length: count }, (_, i) => ({
    id: `player${i}`,
    name: `Student ${i+1}`,
    level: Math.floor(Math.random() * 10) + 1,
    score: Math.floor(Math.random() * 1000),
    timeSpent: (Math.random() * 50).toFixed(1),
    avatar: `https://i.pravatar.cc/150?img=${i+10}`,
    lastActive: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000).toISOString()
  }));
};

const generateClassData = () => {
  return [
    { id: 'class1', name: 'Math 101', students: 15, progress: 68 },
    { id: 'class2', name: 'Algebra II', students: 12, progress: 82 },
    { id: 'class3', name: 'Geometry', students: 18, progress: 54 }
  ];
};

const generatePerformanceData = () => {
  return [
    { subject: 'Addition', score: 85 },
    { subject: 'Subtraction', score: 78 },
    { subject: 'Multiplication', score: 92 },
    { subject: 'Division', score: 65 },
    { subject: 'Equations', score: 88 }
  ];
};

const SmallAvatar = styled(Avatar)(({ theme }) => ({
  width: 22,
  height: 22,
  border: `2px solid ${theme.palette.background.paper}`,
}));

export default function TeacherDashboard() {
  const [players, setPlayers] = useState(generatePlayerData(12));
  const [classes, setClasses] = useState(generateClassData());
  const [activeTab, setActiveTab] = useState(0);
  const [selectedClass, setSelectedClass] = useState(null);
  const [playerDialogOpen, setPlayerDialogOpen] = useState(false);
  const [classDialogOpen, setClassDialogOpen] = useState(false);
  const [newPlayer, setNewPlayer] = useState({ name: '', email: '' });
  const [newClass, setNewClass] = useState({ name: '', description: '' });
  const [anchorEl, setAnchorEl] = useState(null);
  const [selectedPlayer, setSelectedPlayer] = useState(null);

  // Fetch real data from Firebase
  useEffect(() => {
    const playersRef = ref(db, 'players');
    onValue(playersRef, (snapshot) => {
      const data = snapshot.val();
      if (data) {
        setPlayers(Object.keys(data).map(key => ({ id: key, ...data[key] })));
      }
    });

    const classesRef = ref(db, 'classes');
    onValue(classesRef, (snapshot) => {
      const data = snapshot.val();
      if (data) {
        setClasses(Object.keys(data).map(key => ({ id: key, ...data[key] })));
      }
    });
  }, []);

  // Player actions
  const handleAddPlayer = () => {
    const newPlayerRef = push(ref(db, 'players'));
    set(newPlayerRef, {
      ...newPlayer,
      level: 1,
      score: 0,
      joinDate: new Date().toISOString(),
      lastActive: new Date().toISOString()
    });
    setPlayerDialogOpen(false);
    setNewPlayer({ name: '', email: '' });
  };

  const handleResetPassword = (playerId) => {
    update(ref(db, `players/${playerId}`), { 
      needsPasswordReset: true 
    });
  };

  const handleUpdatePlayer = () => {
    // TODO: Implement player update logic
    setPlayerDialogOpen(false);
    setSelectedPlayer(null);
  };

  // Class actions
  const handleAddClass = () => {
    const newClassRef = push(ref(db, 'classes'));
    set(newClassRef, {
      ...newClass,
      students: 0,
      progress: 0,
      createdAt: new Date().toISOString()
    });
    setClassDialogOpen(false);
    setNewClass({ name: '', description: '' });
  };

  const handleUpdateClass = () => {
    // TODO: Implement class update logic
    setClassDialogOpen(false);
    setSelectedClass(null);
  };

  // Data processing
  const progressData = players.map(p => ({
    name: p.name || `Player ${p.id.slice(0,4)}`,
    level: p.level,
    score: p.score,
    timeSpent: parseFloat(p.timeSpent)
  }));

  const classDistribution = classes.map(cls => ({
    name: cls.name,
    value: cls.students
  }));

  const performanceData = generatePerformanceData();

  // Animation variants
  const fadeIn = {
    hidden: { opacity: 0 },
    visible: { opacity: 1, transition: { duration: 0.5 } }
  };

  return (
    <Box sx={{ p: 3, background: '#f5f7fa', minHeight: '100vh' }}>
      {/* Header */}
      <motion.div initial="hidden" animate="visible" variants={fadeIn}>
        <Box sx={{ 
          display: 'flex', 
          justifyContent: 'space-between',
          alignItems: 'center',
          mb: 3
        }}>
          <Typography variant="h4" sx={{ 
            fontWeight: 'bold',
            background: 'linear-gradient(45deg, #3f51b5 30%, #2196f3 90%)',
            WebkitBackgroundClip: 'text',
            WebkitTextFillColor: 'transparent'
          }}>
            Mathio Bros Teacher Dashboard
          </Typography>
          <Box>
            <Button 
              variant="contained" 
              startIcon={<Add />}
              onClick={() => setPlayerDialogOpen(true)}
              sx={{ mr: 2 }}
            >
              Add Student
            </Button>
            <Button 
              variant="outlined" 
              startIcon={<Group />}
              onClick={() => setClassDialogOpen(true)}
            >
              Create Class
            </Button>
          </Box>
        </Box>

        {/* Stats Cards */}
        <Grid container spacing={3} sx={{ mb: 3 }}>
          <Grid item xs={12} md={3}>
            <Card sx={{ 
              background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
              borderRadius: 4
            }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <BarChartIcon sx={{ fontSize: 40, color: '#3f51b5', mr: 2 }} />
                  <Box>
                    <Typography variant="h6">Total Students</Typography>
                    <Typography variant="h4">{players.length}</Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={3}>
            <Card sx={{ 
              background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
              borderRadius: 4
            }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <Class sx={{ fontSize: 40, color: '#3f51b5', mr: 2 }} />
                  <Box>
                    <Typography variant="h6">Active Classes</Typography>
                    <Typography variant="h4">{classes.length}</Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={3}>
            <Card sx={{ 
              background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
              borderRadius: 4
            }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <Timeline sx={{ fontSize: 40, color: '#3f51b5', mr: 2 }} />
                  <Box>
                    <Typography variant="h6">Avg. Progress</Typography>
                    <Typography variant="h4">
                      {Math.round(classes.reduce((acc, cls) => acc + cls.progress, 0) / classes.length)}%
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={3}>
            <Card sx={{ 
              background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)',
              borderRadius: 4
            }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <Star sx={{ fontSize: 40, color: '#3f51b5', mr: 2 }} />
                  <Box>
                    <Typography variant="h6">Top Score</Typography>
                    <Typography variant="h4">
                      {players.reduce((max, p) => p.score > max ? p.score : max, 0)}
                    </Typography>
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        {/* Main tabs */}
        <Paper sx={{ 
          mb: 3, 
          borderRadius: 4,
          boxShadow: '0 8px 16px rgba(0,0,0,0.1)'
        }}>
          <Tabs 
            value={activeTab} 
            onChange={(e, newVal) => setActiveTab(newVal)}
            variant="fullWidth"
            textColor="primary"
            indicatorColor="primary"
          >
            <Tab label="Overview" icon={<Assessment />} />
            <Tab label="Students" icon={<Person />} />
            <Tab label="Classes" icon={<Class />} />
          </Tabs>
        </Paper>

        {/* Tab content */}
        {activeTab === 0 && (
          <Grid container spacing={3}>
            {/* Progress chart */}
            <Grid item xs={12} md={8}>
              <Paper sx={{ 
                p: 3, 
                height: '100%',
                borderRadius: 4,
                boxShadow: '0 4px 12px rgba(0,0,0,0.05)'
              }}>
                <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                  <Timeline color="primary" sx={{ mr: 1 }} />
                  <Typography variant="h6">Class Progress</Typography>
                </Box>
                <ResponsiveContainer width="100%" height={400}>
                  <AreaChart data={progressData}>
                    <defs>
                      <linearGradient id="colorLevel" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor={GRADIENT_COLORS[0]} stopOpacity={0.8}/>
                        <stop offset="95%" stopColor={GRADIENT_COLORS[1]} stopOpacity={0}/>
                      </linearGradient>
                    </defs>
                    <XAxis dataKey="name" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Area 
                      type="monotone" 
                      dataKey="level" 
                      stroke={GRADIENT_COLORS[0]} 
                      fillOpacity={1} 
                      fill="url(#colorLevel)" 
                      name="Level"
                    />
                  </AreaChart>
                </ResponsiveContainer>
              </Paper>
            </Grid>

            {/* Right side charts */}
            <Grid item xs={12} md={4}>
              <Grid container spacing={3}>
                <Grid item xs={12}>
                  <Paper sx={{ 
                    p: 3, 
                    height: '100%',
                    borderRadius: 4,
                    boxShadow: '0 4px 12px rgba(0,0,0,0.05)'
                  }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                      <PieChart color="primary" sx={{ mr: 1 }} />
                      <Typography variant="h6">Class Distribution</Typography>
                    </Box>
                    <ResponsiveContainer width="100%" height={200}>
                      <PieChart>
                        <Pie
                          data={classDistribution}
                          cx="50%"
                          cy="50%"
                          outerRadius={80}
                          fill="#8884d8"
                          dataKey="value"
                          label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
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
                <Grid item xs={12}>
                  <Paper sx={{ 
                    p: 3, 
                    height: '100%',
                    borderRadius: 4,
                    boxShadow: '0 4px 12px rgba(0,0,0,0.05)'
                  }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                      <Assessment color="primary" sx={{ mr: 1 }} />
                      <Typography variant="h6">Skill Radar</Typography>
                    </Box>
                    <ResponsiveContainer width="100%" height={200}>
                      <RadarChart cx="50%" cy="50%" outerRadius="80%" data={performanceData}>
                        <PolarGrid />
                        <PolarAngleAxis dataKey="subject" />
                        <PolarRadiusAxis angle={30} domain={[0, 100]} />
                        <Radar 
                          name="Performance" 
                          dataKey="score" 
                          stroke="#3f51b5" 
                          fill="#3f51b5" 
                          fillOpacity={0.6} 
                        />
                        <Tooltip />
                      </RadarChart>
                    </ResponsiveContainer>
                  </Paper>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
        )}

        {/* Students tab */}
        {activeTab === 1 && (
          <Paper sx={{ 
            p: 3,
            borderRadius: 4,
            boxShadow: '0 4px 12px rgba(0,0,0,0.05)'
          }}>
            <Box sx={{ 
              display: 'flex', 
              justifyContent: 'space-between',
              alignItems: 'center',
              mb: 3
            }}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Person color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6">Student Management</Typography>
              </Box>
              <Button 
                variant="outlined" 
                startIcon={<Refresh />}
                onClick={() => window.location.reload()}
              >
                Refresh
              </Button>
            </Box>
            
            <Grid container spacing={3}>
              {players.map(player => (
                <Grid item xs={12} sm={6} md={4} key={player.id}>
                  <Card sx={{
                    borderRadius: 4,
                    boxShadow: '0 4px 8px rgba(0,0,0,0.1)',
                    transition: 'transform 0.3s',
                    '&:hover': {
                      transform: 'translateY(-5px)'
                    }
                  }}>
                    <CardHeader
                      avatar={
                        <Badge
                          overlap="circular"
                          anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                          badgeContent={
                            <SmallAvatar 
                              alt="Online" 
                              src={player.avatar}
                              sx={{
                                width: 12,
                                height: 12,
                                bgcolor: player.lastActive > new Date(Date.now() - 5*60*1000).toISOString() 
                                  ? '#4caf50' 
                                  : '#f44336'
                              }}
                            />
                          }
                        >
                          <Avatar alt={player.name} src={player.avatar} />
                        </Badge>
                      }
                      action={
                        <IconButton onClick={(e) => {
                          setAnchorEl(e.currentTarget);
                          setSelectedPlayer(player);
                        }}>
                          <MoreVert />
                        </IconButton>
                      }
                      title={player.name}
                      subheader={`Level ${player.level}`}
                    />
                    <CardContent>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                        <Typography variant="body2">Score:</Typography>
                        <Typography variant="body2" fontWeight="bold">{player.score}</Typography>
                      </Box>
                      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
                        <Typography variant="body2">Time Spent:</Typography>
                        <Typography variant="body2" fontWeight="bold">{player.timeSpent}h</Typography>
                      </Box>
                      <LinearProgress 
                        variant="determinate"
                        value={player.progress}
                        sx={{ height: 10, borderRadius: 5 }}
                      />
                    </CardContent>
                  </Card>
                </Grid>
              ))}
            </Grid>

            {/* Player context menu */}
            <Menu
              anchorEl={anchorEl}
              open={Boolean(anchorEl)}
              onClose={() => setAnchorEl(null)}
              PaperProps={{
                elevation: 4,
                sx: { 
                  borderRadius: 4,
                  mt: 1,
                  minWidth: 200
                }
              }}
            >
              <MenuItem 
                onClick={() => {
                  handleResetPassword(selectedPlayer.id);
                  setAnchorEl(null);
                }}
                sx={{ 
                  color: 'text.primary',
                  '&:hover': {
                    backgroundColor: 'rgba(63,81,181,0.1)',
                    color: 'primary.main'
                  }
                }}
              >
                <ListItemIcon>
                  <LockReset fontSize="small" />
                </ListItemIcon>
                Reset Password
              </MenuItem>
              <MenuItem 
                onClick={() => {
                  setSelectedPlayer(null);
                  setAnchorEl(null);
                  setPlayerDialogOpen(true);
                }}
                sx={{ 
                  color: 'text.primary',
                  '&:hover': {
                    backgroundColor: 'rgba(63,81,181,0.1)',
                    color: 'primary.main'
                  }
                }}
              >
                <ListItemIcon>
                  <Edit fontSize="small" />
                </ListItemIcon>
                Edit Details
              </MenuItem>
              <MenuItem 
                onClick={() => {
                  const playerRef = ref(db, `players/${selectedPlayer.id}`);
                  update(playerRef, { 
                    isActive: false 
                  });
                  setAnchorEl(null);
                }}
                sx={{ 
                  color: 'text.primary',
                  '&:hover': {
                    backgroundColor: 'rgba(63,81,181,0.1)',
                    color: 'primary.main'
                  }
                }}
              >
                <ListItemIcon>
                  <Delete fontSize="small" />
                </ListItemIcon>
                Deactivate Account
              </MenuItem>
            </Menu>
          </Paper>
        )}

        {/* Classes tab */}
        {activeTab === 2 && (
          <Paper sx={{ 
            p: 3,
            borderRadius: 4,
            boxShadow: '0 4px 12px rgba(0,0,0,0.05)'
          }}>
            <Box sx={{ 
              display: 'flex', 
              justifyContent: 'space-between',
              alignItems: 'center',
              mb: 3
            }}>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Class color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6">Class Management</Typography>
              </Box>
              <Button 
                variant="outlined" 
                startIcon={<Refresh />}
                onClick={() => window.location.reload()}
              >
                Refresh
              </Button>
            </Box>
            
            <Grid container spacing={3}>
              {classes.map(cls => (
                <Grid item xs={12} sm={6} md={4} key={cls.id}>
                  <Card sx={{
                    borderRadius: 4,
                    boxShadow: '0 4px 8px rgba(0,0,0,0.1)',
                    transition: 'transform 0.3s',
                    '&:hover': {
                      transform: 'translateY(-5px)'
                    }
                  }}>
                    <CardHeader
                      title={cls.name}
                      subheader={`Students: ${cls.students}`}
                    />
                    <CardContent>
                      <Typography variant="body2" sx={{ mb: 2 }}>
                        {cls.description}
                      </Typography>
                      <LinearProgress 
                        variant="determinate"
                        value={cls.progress}
                        sx={{ height: 10, borderRadius: 5, mb: 2 }}
                      />
                      <Button 
                        variant="contained" 
                        size="small" 
                        onClick={() => {
                          setSelectedClass(cls);
                          setClassDialogOpen(true);
                        }}
                        sx={{ 
                          borderRadius: 4,
                          textTransform: 'none',
                          fontSize: 14
                        }}
                      >
                        View Details
                      </Button>
                    </CardContent>
                  </Card>
                </Grid>
              ))}
            </Grid>

            {/* Class dialog */}
            <Dialog
              open={classDialogOpen}
              onClose={() => setClassDialogOpen(false)}
              maxWidth="sm"
              fullWidth
            >
              <DialogTitle>
                {selectedClass ? 'Edit Class' : 'Create Class'}
              </DialogTitle>
              <DialogContent>
                <TextField
                  autoFocus
                  margin="dense"
                  label="Class Name"
                  type="text"
                  fullWidth
                  variant="outlined"
                  value={newClass.name}
                  onChange={(e) => setNewClass({ ...newClass, name: e.target.value })}
                />
                <TextField
                  margin="dense"
                  label="Description"
                  type="text"
                  fullWidth
                  variant="outlined"
                  multiline
                  rows={4}
                  value={newClass.description}
                  onChange={(e) => setNewClass({ ...newClass, description: e.target.value })}
                />
              </DialogContent>
              <DialogActions>
                <Button onClick={() => setClassDialogOpen(false)} color="primary">
                  Cancel
                </Button>
                <Button 
                  onClick={selectedClass ? handleUpdateClass : handleAddClass} 
                  color="primary"
                  variant="contained"
                >
                  {selectedClass ? 'Update Class' : 'Create Class'}
                </Button>
              </DialogActions>
            </Dialog>
          </Paper>
        )}
      </motion.div>

      {/* Player dialog */}
      <Dialog
        open={playerDialogOpen}
        onClose={() => setPlayerDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          {selectedPlayer ? 'Edit Student' : 'Add Student'}
        </DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            label="Full Name"
            type="text"
            fullWidth
            variant="outlined"
            value={newPlayer.name}
            onChange={(e) => setNewPlayer({ ...newPlayer, name: e.target.value })}
          />
          <TextField
            margin="dense"
            label="Email Address"
            type="email"
            fullWidth
            variant="outlined"
            value={newPlayer.email}
            onChange={(e) => setNewPlayer({ ...newPlayer, email: e.target.value })}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPlayerDialogOpen(false)} color="primary">
            Cancel
          </Button>
          <Button 
            onClick={selectedPlayer ? handleUpdatePlayer : handleAddPlayer} 
            color="primary"
            variant="contained"
          >
            {selectedPlayer ? 'Update Student' : 'Add Student'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
