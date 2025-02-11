import { useLocation, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";
import { API_URL } from "./constants.ts";
import axios from "axios";
import { GameMessage } from "./types/gameMessage.ts";

export const Game = () => {
  const location = useLocation();
  const [chatHistory, setChatHistory] = useState<GameMessage[]>([]);
  const [isWatcher, setIsWatcher] = useState(true);
  const [action1, setAction1] = useState("");
  const [action2, setAction2] = useState("");
  const [result, setResult] = useState("");
  const [isRoundEnded, setIsRoundEnded] = useState(false);
  const [countdown, setCountdown] = useState(3);
  const [connection, setConnection] = useState<HubConnection>();
  const [isChoosed, setIsChoosed] = useState(false);
  const navigate = useNavigate();
  const [chatInput, setChatInput] = useState("");
  console.log(chatHistory);
  const onJoinGame = (role: number) => {
    setIsWatcher(role == 1);
    axios
      .get(`${API_URL}/game/${location.state.gameId}`, {
        headers: { Authorization: "Bearer " + localStorage.getItem("token") },
      })
      .then((resp) => {
        setChatHistory(resp.data.messages);
      });
  };
  const MessageReceive = (msg: GameMessage) => {
    setChatHistory((prev) => [...prev, msg]);
  };
  const onOtherMove = () => {
    console.log("move");
  };
  const onResultRecv = (data) => {
    setAction1(data.winnerFigure);
    setAction2(data.loserFigure);
    setResult(data.message);
    setIsRoundEnded(true);
  };
  const onError = (msg) => {
    setChatHistory((prev) => [...prev, msg]);
  };
  const submitAction = (act: string) => {
    if (connection) {
      connection
        .invoke(
          "SubmitAction",
          location.state.gameId,
          act === "Stone" ? 0 : act === "Scissors" ? 1 : 2,
        )
        .then(() => {
          setIsChoosed(true);
        });
    }
  };
  useEffect(() => {
    const token = localStorage.getItem("token");
    const connectionInstanse = new HubConnectionBuilder()
      .withUrl(`${API_URL}/game`, {
        accessTokenFactory(): string | Promise<string> {
          return token || "";
        },
        transport: HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .withAutomaticReconnect()
      .build();
    connectionInstanse.on("SuccessJoin", onJoinGame);
    connectionInstanse.on("MessageReceive", MessageReceive);
    connectionInstanse.on("ResultReceive", onResultRecv);
    connectionInstanse.on("AnotherPlayerMadeMove", onOtherMove);
    connectionInstanse.on("SomethingWentWrong", onError);
    connectionInstanse.start().then(() => {
      connectionInstanse.invoke("JoinGame", location.state.gameId).then(() => {
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
      setIsChoosed(false);
    }
  }, [countdown, isRoundEnded]);
  return (
    <>
      <header>
        <button
          onClick={() => {
            navigate("/games");
          }}
        >
          Главная
        </button>
      </header>
      <div className={"flex flex-row"}>
        <div>
          {chatHistory.map((message, index) => (
            <div key={index}>
              {message.from}:{message.text}
            </div>
          ))}
          <div className={"flex flex-row"}>
            <input
              onChange={(e) => {
                setChatInput(e.target.value);
              }}
            />
            <button
              onClick={() => {
                if (connection)
                  connection
                    .invoke(
                      "SendMessageToChat",
                      location.state.gameId,
                      chatInput,
                    )
                    .then(() => {
                      console.log("send");
                    });
              }}
            >
              Отправить
            </button>
          </div>
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
