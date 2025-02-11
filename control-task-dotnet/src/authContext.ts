import React, { Dispatch, SetStateAction } from "react";

export const AuthContext = React.createContext<{
  userId: string;
  isAuthenticated: boolean;
  setIsAuthenticated: Dispatch<SetStateAction<boolean>>;
  login: () => void;
  logout: () => void;
}>({
  userId: "",
  isAuthenticated: false,
  setIsAuthenticated: () => {},
  login: () => {},
  logout: () => {},
});
