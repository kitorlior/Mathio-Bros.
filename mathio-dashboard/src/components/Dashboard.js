import React, { useState, useEffect } from 'react';
import { ref, onValue, update, push, set } from 'firebase/database';
import { db } from '../FireBaseDB';
import { 
  Box, Typography, Paper, Grid, Button, 
  Tabs, Tab, IconButton, Dialog, DialogTitle, 
  DialogContent, TextField, DialogActions,
  Avatar, LinearProgress, Badge,
  Card, CardContent, CardHeader, Menu, MenuItem, 
  ListItemIcon
} from '@mui/material';
import {
  BarChart as BarChartIcon, Timeline, Assessment, Group, Star,
  Person, Class, Edit, Add, Delete, Refresh, LockReset, MoreVert
} from '@mui/icons-material';
import { motion } from 'framer-motion';
import { styled } from '@mui/material/styles';
import {
  ResponsiveContainer, AreaChart, Area, XAxis, YAxis, Tooltip, Legend,
  PieChart, Pie, Cell,
  RadarChart, PolarGrid, PolarAngleAxis, PolarRadiusAxis, Radar
} from 'recharts';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8'];
const GRADIENT_COLORS = ['#3f51b5', '#2196f3'];

const SmallAvatar = styled(Avatar)(({ theme }) => ({
  width: 22,
  height: 22,
  border: `2px solid ${theme.palette.background.paper}`,
}));

export default function TeacherDashboard() {
  // State
  const [players, setPlayers] = useState([]);
  const [classes, setClasses] = useState([]);
  const [activeTab, setActiveTab] = useState(0);
  const [selectedClass, setSelectedClass] = useState(null);
  const [playerDialogOpen, setPlayerDialogOpen] = useState(false);
  const [classDialogOpen, setClassDialogOpen] = useState(false);
  const [newPlayer, setNewPlayer] = useState({ name: '', email: '', class: '' });
  const [newClass, setNewClass] = useState({ name: '', description: '', teacher: '', students: 0 });
  const [anchorEl, setAnchorEl] = useState(null);
  const [selectedPlayer, setSelectedPlayer] = useState(null);
  const [loading, setLoading] = useState(true);
  const [playersLoading, setPlayersLoading] = useState(true);
  const [classesLoading, setClassesLoading] = useState(true);

  // Fetch real data from Firebase
  useEffect(() => {
    const playersRef = ref(db, 'players');
    const classesRef = ref(db, 'classes');

    const unsubPlayers = onValue(playersRef, (snapshot) => {
      const data = snapshot.val();
      setPlayers(data ? Object.keys(data).map(key => ({ id: key, ...data[key] })) : []);
      setPlayersLoading(false);
    });

    const unsubClasses = onValue(classesRef, (snapshot) => {
      const data = snapshot.val();
      setClasses(data ? Object.keys(data).map(key => ({ id: key, ...data[key] })) : []);
      setClassesLoading(false);
    });

    return () => {
      unsubPlayers();
      unsubClasses();
    };
  }, []);

  // Player actions
  const handleAddPlayer = async () => {
    const newPlayerRef = push(ref(db, 'players'));
    await set(newPlayerRef, {
      ...newPlayer,
      level: 1,
      score: 0,
      joinDate: new Date().toISOString(),
      lastActive: new Date().toISOString(),
      progress: 0,
      isActive: true,
      avatar: `https://i.pravatar.cc/150?u=${newPlayer.email || Math.random()}`
    });
    setPlayerDialogOpen(false);
    setNewPlayer({ name: '', email: '', class: '' });
    setSelectedPlayer(null);
  };

  const handleUpdatePlayer = async () => {
    if (!selectedPlayer) return;
    const playerRef = ref(db, `players/${selectedPlayer.id}`);
    await update(playerRef, {
      ...selectedPlayer,
      ...newPlayer
    });
    setPlayerDialogOpen(false);
    setSelectedPlayer(null);
    setNewPlayer({ name: '', email: '', class: '' });
  };

  const handleResetPassword = async (playerId) => {
    await update(ref(db, `players/${playerId}`), { 
      needsPasswordReset: true 
    });
  };

  // Class actions
  const handleAddClass = async () => {
    const newClassRef = push(ref(db, 'classes'));
    await set(newClassRef, {
      ...newClass,
      students: 0,
      progress: 0,
      createdAt: new Date().toISOString()
    });
    setClassDialogOpen(false);
    setNewClass({ name: '', description: '', teacher: '', students: 0 });
    setSelectedClass(null);
  };

  const handleUpdateClass = async () => {
    if (!selectedClass) return;
    const classRef = ref(db, `classes/${selectedClass.id}`);
    await update(classRef, {
      ...selectedClass,
      ...newClass
    });
    setClassDialogOpen(false);
    setSelectedClass(null);
    setNewClass({ name: '', description: '', teacher: '', students: 0 });
  };

  // Dialog openers
  const openEditPlayerDialog = (player) => {
    setSelectedPlayer(player);
    setNewPlayer({ name: player.name, email: player.email, class: player.class || '' });
    setPlayerDialogOpen(true);
  };

  const openEditClassDialog = (cls) => {
    setSelectedClass(cls);
    setNewClass({ name: cls.name, description: cls.description || '', teacher: cls.teacher || '', students: cls.students || 0 });
    setClassDialogOpen(true);
  };

  // Data processing for charts
  const progressData = players.map(p => ({
    name: p.name || `Player ${p.id?.slice(0,4)}`,
    level: p.level || 0,
    score: p.score || 0,
    timeSpent: parseFloat(p.timeSpent || 0)
  }));

  const classDistribution = classes.map(cls => ({
    name: cls.name || '',
    value: cls.students || 0
  }));

  const performanceData = [
    { subject: 'Addition', score: 85 },
    { subject: 'Subtraction', score: 90 },
    { subject: 'Multiplication', score: 75 },
    { subject: 'Division', score: 80 },
    { subject: 'Algebra', score: 70 }
  ];

  // Animation variants
  const fadeIn = {
    hidden: { opacity: 0 },
    visible: { opacity: 1, transition: { duration: 0.5 } }
  };

  if (playersLoading || classesLoading) return <div>Loading...</div>;

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
              onClick={() => {
                setSelectedPlayer(null);
                setNewPlayer({ name: '', email: '', class: '' });
                setPlayerDialogOpen(true);
              }}
              sx={{ mr: 2 }}
            >
              Add Student
            </Button>
            <Button 
              variant="outlined" 
              startIcon={<Group />}
              onClick={() => {
                setSelectedClass(null);
                setNewClass({ name: '', description: '', teacher: '', students: 0 });
                setClassDialogOpen(true);
              }}
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
  <Grid 
    container 
    spacing={4} 
    sx={{ width: '100%', m: 0 }}
    justifyContent="center" 
    alignItems="stretch"
  >
    {/* Progress Chart */}
    <Grid item xs={12} md={6} lg={4} sx={{ display: 'flex' }}>
      <Paper sx={{
        p: 3,
        borderRadius: 4,
        boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
        flex: 1,
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        minWidth: 300,
      }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Timeline color="primary" sx={{ mr: 1 }} />
          <Typography variant="h6">Class Progress</Typography>
        </Box>
        <ResponsiveContainer width="100%" height={300}>
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

    {/* Class Distribution */}
    <Grid item xs={12} md={6} lg={4} sx={{ display: 'flex' }}>
      <Paper sx={{
        p: 3,
        borderRadius: 4,
        boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
        flex: 1,
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        minWidth: 300,
      }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <PieChart color="primary" sx={{ mr: 1 }} />
          <Typography variant="h6">Class Distribution</Typography>
        </Box>
        <ResponsiveContainer width="100%" height={300}>
          <PieChart>
            <Pie
              data={classDistribution}
              cx="50%"
              cy="50%"
              outerRadius={100}
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

    {/* Skill Radar */}
    <Grid item xs={12} md={6} lg={4} sx={{ display: 'flex' }}>
      <Paper sx={{
        p: 3,
        borderRadius: 4,
        boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
        flex: 1,
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        minWidth: 300,
      }}>
        <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
          <Assessment color="primary" sx={{ mr: 1 }} />
          <Typography variant="h6">Skill Radar</Typography>
        </Box>
        <ResponsiveContainer width="100%" height={300}>
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
)}

        {/* Students tab */}
        {activeTab === 1 && (
          <Paper sx={{ p: 3, borderRadius: 4, boxShadow: '0 4px 12px rgba(0,0,0,0.05)' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
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
                    '&:hover': { transform: 'translateY(-5px)' }
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
                          <Avatar alt={player.name} src={player.avatar || ''} />
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
                        <Typography variant="body2">Class:</Typography>
                        <Typography variant="body2" fontWeight="bold">{player.class}</Typography>
                      </Box>
                      <LinearProgress 
                        variant="determinate"
                        value={player.progress || 0}
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
                sx: { borderRadius: 4, mt: 1, minWidth: 200 }
              }}
            >
              <MenuItem 
                onClick={() => {
                  handleResetPassword(selectedPlayer.id);
                  setAnchorEl(null);
                }}
              >
                <ListItemIcon>
                  <LockReset fontSize="small" />
                </ListItemIcon>
                Reset Password
              </MenuItem>
              <MenuItem 
                onClick={() => {
                  openEditPlayerDialog(selectedPlayer);
                  setAnchorEl(null);
                }}
              >
                <ListItemIcon>
                  <Edit fontSize="small" />
                </ListItemIcon>
                Edit Details
              </MenuItem>
              <MenuItem 
                onClick={async () => {
                  if (selectedPlayer) {
                    const playerRef = ref(db, `players/${selectedPlayer.id}`);
                    await update(playerRef, { isActive: false });
                  }
                  setAnchorEl(null);
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
          <Paper sx={{ p: 3, borderRadius: 4, boxShadow: '0 4px 12px rgba(0,0,0,0.05)' }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
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
                    '&:hover': { transform: 'translateY(-5px)' }
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
                        value={cls.progress || 0}
                        sx={{ height: 10, borderRadius: 5, mb: 2 }}
                      />
                      <Button 
                        variant="contained" 
                        size="small" 
                        onClick={() => openEditClassDialog(cls)}
                        sx={{ borderRadius: 4, textTransform: 'none', fontSize: 14 }}
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
                <TextField
                  margin="dense"
                  label="Teacher"
                  type="text"
                  fullWidth
                  variant="outlined"
                  value={newClass.teacher}
                  onChange={(e) => setNewClass({ ...newClass, teacher: e.target.value })}
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
          <TextField
            margin="dense"
            label="Class"
            type="text"
            fullWidth
            variant="outlined"
            value={newPlayer.class}
            onChange={(e) => setNewPlayer({ ...newPlayer, class: e.target.value })}
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
