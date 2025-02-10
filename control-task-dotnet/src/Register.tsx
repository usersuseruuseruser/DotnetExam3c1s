import React, { useState } from "react";
import { AuthContext } from "./authContext.ts";
import axios from "axios";
import { API_URL } from "./constants.ts";

export const Register = () => {
  const { login } = React.useContext(AuthContext);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [repeatedPassword, setRepeatedPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async () => {
    try {
      if (password !== repeatedPassword) {
        setError(`Пароли не совпадают`);
        return;
      }
      if (password === "") {
        setError(`Пароль пустой`);
        return;
      }
      const response = await axios.post(`${API_URL}/register`, {
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
      <h1>Register</h1>
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
      <input
        type="password"
        value={repeatedPassword}
        onChange={(e) => setRepeatedPassword(e.target.value)}
        placeholder="Password"
      />
      <button onClick={handleLogin}>Register</button>
    </div>
  );
};
