import { useState } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  FormControl,
  InputLabel,
  Input,
  Button,
  Typography,
  Box,
} from "@mui/material";
import { login } from "./authSlice";
import { AppDispatch } from "../../global-store/store";

const UserLogin = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const shouldLoginButtonBeDisabled = () => {
    return email === "" || password === "";
  };

  const clearError = () => {
    setError("");
  };

  const handleLogin = async () => {
    try {
      await dispatch(login({ email, password })).unwrap();
      clearError();
      navigate("/");
    } catch (error) {
      setError("Username or password is wrong. Please try again.");
    }
  };

  return (
    <Box>
      <FormControl fullWidth margin="normal">
        <InputLabel htmlFor="email">Email</InputLabel>
        <Input
          id="email"
          type="email"
          value={email}
          onChange={(e) => {
            setEmail(e.target.value);
            clearError();
          }}
        />
      </FormControl>

      <FormControl fullWidth margin="normal">
        <InputLabel htmlFor="password">Password</InputLabel>
        <Input
          id="password"
          type="password"
          value={password}
          onChange={(e) => {
            setPassword(e.target.value);
            clearError();
          }}
        />
      </FormControl>

      {error && <Typography color="error">{error}</Typography>}

      <Button
        variant="contained"
        color="primary"
        onClick={handleLogin}
        disabled={shouldLoginButtonBeDisabled()}
      >
        Login
      </Button>
    </Box>
  );
};

export default UserLogin;
