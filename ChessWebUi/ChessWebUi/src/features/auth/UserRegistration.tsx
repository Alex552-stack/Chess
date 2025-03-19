import { useState } from "react";
import { useDispatch } from "react-redux";
import {
  FormControl,
  InputLabel,
  Input,
  Button,
  Typography,
  Box,
} from "@mui/material";
import { register } from "./authSlice";
import { AppDispatch } from "../../global-store/store";
import { useNavigate } from "react-router-dom";

const UserRegistration = () => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");

  const shouldRegisterButtonBeDisabled = () => {
    return (
      email === "" || password === "" || confirmPassword === "" || error !== ""
    );
  };

  const validEmailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  const validateEmail = (email: string) => {
    return validEmailRegex.test(email);
  };

  const clearError = () => {
    setError("");
  };

  const handleRegister = async () => {
    if (!validateEmail(email)) {
      setError("Invalid email format.");
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }
    try {
      await dispatch(register({ email, password })).unwrap();

      clearError();
      navigate("/");
    } catch (error: any) {
      console.log(error);

      navigate("/error", {
        state: { message: error.message || "Registration failed." },
      });
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

      <FormControl fullWidth margin="normal">
        <InputLabel htmlFor="confirm-password">Confirm Password</InputLabel>
        <Input
          id="confirm-password"
          type="password"
          value={confirmPassword}
          onChange={(e) => {
            setConfirmPassword(e.target.value);
            clearError();
          }}
        />
      </FormControl>

      {error && <Typography color="error">{error}</Typography>}

      <Button
        variant="contained"
        color="primary"
        onClick={handleRegister}
        disabled={shouldRegisterButtonBeDisabled()}
      >
        Register
      </Button>
    </Box>
  );
};

export default UserRegistration;
