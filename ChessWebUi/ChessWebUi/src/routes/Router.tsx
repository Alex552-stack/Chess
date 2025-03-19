import { createBrowserRouter } from "react-router-dom";
import AppLayout from "./AppLayout";
import ErrorPage from "../pages/ErrorPage";
import Main from "../pages/Main";
import AuthPage from "../pages/AuthPage";
import GamePage from "../pages/GamePage.tsx";

const Router = createBrowserRouter(
  [
    {
      path: "/",
      element: <AppLayout />,
      errorElement: <ErrorPage />,
      children: [
        {
          path: "/",
          element: <Main />,
        },
        {
          path: "/game",
          element: <GamePage/>
        },
        {
          path: "/auth",
          element: <AuthPage />,
        },
        {
          path: "error",
          element: <ErrorPage />,
        },
      ],
    },
  ],
  {
    future: {
      v7_normalizeFormMethod: true,
      v7_fetcherPersist: true,
      v7_partialHydration: true,
      v7_skipActionErrorRevalidation: true,
      v7_relativeSplatPath: true,
    },
  }
);

export default Router;
