import { Provider, useDispatch } from "react-redux";
import { RouterProvider } from "react-router-dom";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import { useEffect } from "react";
import { setUser } from "./features/auth/authSlice";
import { decodeToken } from "./features/auth/authHelper";
import router from "./routes/Router";
import store from "./global-store/store";

const darkTheme = createTheme({
  palette: {
    mode: "dark",
  },
});

const App = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    const decodedToken = decodeToken();
    if (decodedToken) {
      dispatch(setUser(decodedToken));
    }
  }, [dispatch]);

  return (
    <Provider store={store}>
      <ThemeProvider theme={darkTheme}>
        <CssBaseline />
        <RouterProvider router={router} />
      </ThemeProvider>
    </Provider>
  );
};

export default App;
