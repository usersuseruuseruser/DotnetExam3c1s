import React from "react";

export const AuthContext = React.createContext<{
  userId: string;
  isAuthenticated: boolean;
  login: () => void;
  logout: () => void;
}>({
  userId: "",
  isAuthenticated: false,
  login: () => {},
  logout: () => {},
});
