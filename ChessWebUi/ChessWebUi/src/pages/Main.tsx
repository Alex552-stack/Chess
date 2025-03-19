import { Link } from "react-router-dom";
import Counter from "../features/counter/Counter";
import { useSelector } from "react-redux";
import { RootState } from "../global-store/store";
import { useEffect } from "react";

export default function Main() {
  const user = useSelector((state: RootState) => state.auth.user);

  const capitalizeFirstLetter = (text: string) => {
    return text.charAt(0).toUpperCase() + text.slice(1);
  };

  useEffect(() => {
    console.log(user);
  }, [user]);

  return (
    <>
      <h1>it works bro</h1>
      <ul>
        {user &&
          Object.entries(user).map(([key, value]) => (
            <li key={key}>
              <strong>{capitalizeFirstLetter(key)}:</strong> {String(value)}
            </li>
          ))}
      </ul>
      <a href="/nolongerworksbro">Link</a>
      <Link to={"/nolongerworksbro"}>Link Reacrt</Link>
      <Counter />
    </>
  );
}
