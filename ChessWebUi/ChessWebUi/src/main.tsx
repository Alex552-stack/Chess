import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./App.css";

import { Provider } from "react-redux";
import store from "./global-store/store";
import App from "./App";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </StrictMode>
);
