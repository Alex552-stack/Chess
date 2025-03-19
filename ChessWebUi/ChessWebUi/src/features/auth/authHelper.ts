import { jwtDecode } from "jwt-decode";
import { User } from "../../models/User";

const TOKEN_KEY = "token";

export const setToken = (token: string): void => {
  localStorage.setItem(TOKEN_KEY, token);
};

export const getToken = (): string | null => {
  return localStorage.getItem(TOKEN_KEY);
};

export const removeToken = (): void => {
  localStorage.removeItem(TOKEN_KEY);
};

export const decodeToken = (): User | null => {
  const token = getToken();
  if (token) {
    const decoded: any = jwtDecode(token);
    return {
      email: decoded.email,
      roles: JSON.parse(decoded.roles),
      scope: decoded.scope,
      businesses: JSON.parse(decoded.businesses),
    };
  }
  return null;
};
