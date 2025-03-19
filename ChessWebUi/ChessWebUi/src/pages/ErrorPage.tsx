import { useRouteError, useLocation } from "react-router-dom";

export default function ErrorPage() {
  const error = useRouteError() as
    | { statusText?: string; message?: string }
    | undefined;
  const location = useLocation();
  const state = location.state as { message?: string } | undefined;

  const message =
    error?.statusText ||
    error?.message ||
    state?.message ||
    "An unexpected error has occurred.";

  console.error(error || state?.message);

  return (
    <div id="error-page">
      <h1>Oops!</h1>
      <p>Sorry, an unexpected error has occurred.</p>
      <p>
        <i>{message}</i>
      </p>
    </div>
  );
}
