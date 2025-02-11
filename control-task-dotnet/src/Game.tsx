import { useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { API_URL } from "./constants.ts";

export const Game = () => {
  const location = useLocation();
  const [chatHistory, setChatHistory] = useState<string[]>([]);
  const [isWatcher, setIsWatcher] = useState(true);
  const [action1, setAction1] = useState("");
  const [action2, setAction2] = useState("");
  const [result, setResult] = useState("");
  const [isRoundEnded, setIsRoundEnded] = useState(false);
  const [countdown, setCountdown] = useState(3);
  const [connection, setConnection] = useState<HubConnection>();
  const [isChoosed, setIsChoosed] = useState(false);
  const onJoinGame = (ownerId: string, playerId: string, chat: string[]) => {
    setChatHistory(chat);
    setIsWatcher(
      location.state.userId !== ownerId && location.state.userId !== playerId,
    );
  };
  const onMessageRecv = (message: string) => {
    setChatHistory([...chatHistory, message]);
  };
  const onResultRecv = (action1: string, action2: string, result: string) => {
    setAction1(action1);
    setAction2(action2);
    setResult(result);
    setIsRoundEnded(true);
  };
  const submitAction = (act: string) => {
    if (connection) {
      connection.invoke("SubmitAction", location.state.userId, act).then(() => {
        setIsChoosed(true);
      });
    }
  };
  useEffect(() => {
    const connectionInstanse = new HubConnectionBuilder()
      .withUrl(`${API_URL}/games`)
      .withAutomaticReconnect()
      .build();
    connectionInstanse.on("onJoinGame", onJoinGame);
    connectionInstanse.on("onMessageRecv", onMessageRecv);
    connectionInstanse.on("onResultRecv", onResultRecv);
    connectionInstanse.start().then(() => {
      connectionInstanse
        .invoke("JoinGame", location.state.userId, location.state.gameId)
        .then(() => {
          setConnection(connectionInstanse);
          console.log("Joined");
        });
    });
    return () => {
      connectionInstanse.stop().then(() => {
        console.log("Connection stopped");
      });
    };
  }, []);
  useEffect(() => {
    if (isRoundEnded && countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    } else if (isRoundEnded && countdown === 0) {
      setCountdown(3);
      setIsRoundEnded(false);
    }
  }, [countdown, isRoundEnded]);
  return (
    <>
      <header>
        <button>Главная</button>
      </header>
      <div className={"flex flex-row"}>
        <div>
          {chatHistory.map((message, index) => (
            <div key={index}>{message}</div>
          ))}
        </div>
        <div>
          <div>Игрок1</div>
          {isRoundEnded && <div>00:0{countdown}</div>}
          <hr />
          {isRoundEnded && (
            <div>{`Игрок1 выбрал ${action1}, Игрок2 выбрал ${action2}. Результат: ${result}`}</div>
          )}

          {!isWatcher && !isChoosed && (
            <div>
              <button
                onClick={() => {
                  submitAction("Stone");
                }}
              >
                Камень
              </button>
              <button
                onClick={() => {
                  submitAction("Scissors");
                }}
              >
                Ножницы
              </button>
              <button
                onClick={() => {
                  submitAction("Paper");
                }}
              >
                Бумага
              </button>
            </div>
          )}
          <div>{isWatcher ? "Игрок2" : "Ты"}</div>
        </div>
      </div>
    </>
  );
};
