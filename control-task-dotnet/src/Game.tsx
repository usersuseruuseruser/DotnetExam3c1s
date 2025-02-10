import { useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { API_URL } from "./constants.ts";

export const Game = () => {
  const location = useLocation();
  const [connection, setConnection] = useState({});
  const [chatHistory, setChatHistory] = useState<string[]>([]);
  const [isWatcher, setIsWatcher] = useState(true);
  const onJoinGame = (ownerId: string, playerId: string, chat: string[]) => {
    setChatHistory(chat);
    setIsWatcher(
      location.state.userId !== ownerId && location.state.userId !== playerId,
    );
  };
  useEffect(() => {
    const connectionInstanse = new HubConnectionBuilder()
      .withUrl(`${API_URL}/games`)
      .withAutomaticReconnect()
      .build();
    connectionInstanse.on("onJoinGame", onJoinGame);
    connectionInstanse.start().then(() => {
      connectionInstanse
        .invoke("JoinGame", location.state.userId, location.state.gameId)
        .then(() => {
          setConnection(connectionInstanse);
          console.log("Joined");
        });
    });
  }, []);
  return <h1>Game Page</h1>;
};
