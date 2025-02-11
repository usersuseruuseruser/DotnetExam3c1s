import React, { JSX } from "react";
import { Navigate } from "react-router-dom";

export const ProtectedRoute: React.FC<{ element: JSX.Element }> = ({
  element,
}) => {
  const isAuthenticated = localStorage.getItem("token");
  return isAuthenticated ? element : <Navigate to="/" />;
};
