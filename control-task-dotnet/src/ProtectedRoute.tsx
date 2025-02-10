import React, {JSX} from "react";
import {Navigate} from "react-router-dom";
import {AuthContext} from "./authContext.ts";

export const ProtectedRoute: React.FC<{ element: JSX.Element }> = ({ element }) => {
    const { isAuthenticated } = React.useContext(AuthContext);
    return isAuthenticated ? element : <Navigate to="/" />;
};