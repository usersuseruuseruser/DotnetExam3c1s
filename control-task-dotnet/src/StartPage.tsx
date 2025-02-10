import axios from "axios";
import { API_URL } from "./constants.ts";
import React, { useState } from "react";
import { AuthContext } from "./authContext.ts";

export const StartPage = () => {
  const { login } = React.useContext(AuthContext);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async () => {
    try {
      if (password === "") {
        setError(`Пароль пустой`);
        return;
      }
      if (username === "") {
        setError(`Имя пользователя пустое`);
        return;
      }
      const response = await axios.post(`${API_URL}/login`, {
        username,
        password,
      });
      localStorage.setItem("token", response.data.token);
      login();
    } catch (err) {
      setError(`Invalid credentials ${err}`);
    }
  };

  return (
    <div>
      <h1>Login</h1>
      {error && <p style={{ color: "red" }}>{error}</p>}
      <input
        type="text"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
        placeholder="Username"
      />
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
      />
      <button onClick={handleLogin}>Login</button>
      <a href="/register">Register</a>
    </div>
  );
};
