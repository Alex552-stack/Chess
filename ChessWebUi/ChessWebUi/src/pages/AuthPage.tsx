import React, { useState } from "react";
import { Container, Box, Typography, Tabs, Tab } from "@mui/material";
import UserLogin from "../features/auth/UserLogin";
import UserRegistration from "../features/auth/UserRegistration";

const AuthPage = () => {
  const [tabIndex, setTabIndex] = useState(0);

  const handleTabChange = (event: React.ChangeEvent<{}>, newValue: number) => {
    setTabIndex(newValue);
  };

  return (
    <Container maxWidth="sm">
      <Box sx={{ mt: 8 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          {tabIndex === 0 ? "Login" : "Register"}
        </Typography>

        <Tabs value={tabIndex} onChange={handleTabChange} centered>
          <Tab label="Login" />
          <Tab label="Register" />
        </Tabs>

        {tabIndex === 0 && <UserLogin />}
        {tabIndex === 1 && <UserRegistration />}
      </Box>
    </Container>
  );
};

export default AuthPage;
