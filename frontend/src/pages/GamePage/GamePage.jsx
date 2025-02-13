import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import Confetti from 'react-confetti';
import useWindowSize from './useWindowResize';
import SignalRService from './../../services/signalrService';
import MovePanel from './../../components/MovePanel/MovePanel';
import './GamePage.css';

const GamePage = () => {
  const { roomId } = useParams();
  const { width, height } = useWindowSize();
  const [isPlayer, setIsPlayer] = useState(false);
  const [madeMove, setMadeMove] = useState(false);
  const [isActive, setIsActive] = useState(false);
  const [showConfetti, setShowConfetti] = useState(true);
  const [canJoin, setCanJoin] = useState(false);
  const [playerOne, setPlayerOne] = useState(null);
  const [playerTwo, setPlayerTwo] = useState(null);
  const [winner, setWinner] = useState(null);
  const [winnerLoading, setWinnerLoading] = useState(false);
  const [leaving, setLeaving] = useState(false);
  const [countdown, setCountdown] = useState(null);
  const [service, setService] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const signalRService = new SignalRService();
    setService(signalRService);

    // подключение не к руме, а к игре!
    // к руме подключаются спектаторы, к игре - игроки
    signalRService.on('OnUserConnected', (data) => {
      if (data.playerName && !playerOne && (!playerTwo || (playerTwo && playerTwo.name !== data.playerName))) {} 
        setPlayerOne(prev => ({ ...prev, name: data.playerName }));
      if (data.playerName && !playerTwo && (!playerOne || (playerOne && playerOne.name !== data.playerName))) 
        setPlayerTwo(prev => ({ ...prev, name: data.playerName }));

      toast.success(`Игрок ${data.playerName} подключился. Игра началась!`)      
    });

    // отключение от игры, а не от румы!
    // при отключении автоматически становимся спектаторами
    signalRService.on('OnUserDisconnected', (data) => {
      if (data.playerName && data.playerName === playerOne.name)
        setPlayerOne(null)
      if (data.playerName && data.playerName === playerTwo.name)
        setPlayerTwo(null)

      toast.info(`Игрок ${data.playerName} отключился.`);
      if (playerOne || playerTwo) setIsActive(true)
      if (playerOne && !playerTwo ||
          playerTwo && !playerOne) setCanJoin(true)
    });

    signalRService.on('OnMoveMade', (data) => {
      if (playerOne.name === data.playerName) {
        setPlayerOne(prev => ({ ...prev, madeMove: true }));
        if (madeMove) setWinnerLoading(true)
      }
      
      if (playerTwo.name === data.playerName) {
        setPlayerTwo(prev => ({ ...prev, madeMove: true }));
        if (madeMove) setWinnerLoading(true)
      }

      if (playerOne.madeMove && playerTwo.madeMove) setIsActive(false)
    });

    signalRService.on('OnWinnerCalculated', (data) => {
      const moves = data.moves;
      if (moves) {
        Object.keys(moves).forEach((name) => {
          if (name === playerOne.name) playerOne.madeMove = moves[name]
          if (name === playerTwo.name) playerTwo.madeMove = moves[name]
        })
      }
      setIsActive(false)
      setWinner(data.winnerName)
      setWinnerLoading(false)
      setShowConfetti(true)

      const nextGameStartTime = new Date(data.nextGameStartTime).getTime();
      const currentTime = Date.now();
      let sleepTime = nextGameStartTime - currentTime - 5000;
      if (sleepTime < 0) sleepTime = 0;

      setTimeout(() => {
        let count = 5;
        const intervalId = setInterval(() => {
          setCountdown(count);
          count--;
          if (count < 0) {
            setCanJoin(true)
            setIsActive(true)
            clearInterval(intervalId);
            setCountdown("Начинаем!");
            handleJoinGame();
            setWinner('');
            setCountdown(null);
          }
        }, 1000);
      }, sleepTime);
    });

    signalRService
      .startConnection()
      .then(() => {
        signalRService.joinRoom(roomId)
            .then((result) => {
              setCanJoin(result.joinGame);
              setIsActive(true)
              if (result.playerOne) setPlayerOne(prev => ({ ...prev, name: result.playerOne }));
              if (result.playerTwo) setPlayerTwo(prev => ({ ...prev, name: result.playerTwo }));
              if (result.winner) {
                setIsActive(false)
                setWinner(result.winner)
              }
            })
            .catch((error) => {
              toast.error(`Ошибка при подключении к комнате: ${error.message}`);
            });
      })

    return () => {
      signalRService.off('OnUserConnected');
      signalRService.off('OnUserDisconnected');
      signalRService.off('OnMoveMade');
      signalRService.off('OnWinnerCalculated');
    };
  }, [roomId]);

  const handleJoinGame = () => {
    if (!canJoin) {
      toast.error('Игра уже началась')
      return;
    }

    if (!isActive) {
      toast.info('Игра окончена\nПодождите следующего раунда или перейдите в другую игру.')
      return;
    }

    service
      .joinGame(roomId)
      .then((result) => {
        if (result.isSuccess) {
          if (!playerOne) setPlayerOne({madeMove: false})
          else if (!playerTwo) setPlayerTwo({madeMove: false})
          setIsPlayer(true);
          setIsActive(true);
          setCanJoin(false);
          toast.success('Вы успешно присоединились к игре!');
        } else {
          toast.error(`Не удалось присоединиться: ${result.errorMessage}`);
        }
      })
      .catch((error) => {
        toast.error(`Ошибка JoinGame: ${error.message}`);
      });
  };

  const handleWin = (data) => {
    setIsActive(false)
    setWinner(data.winnerName)
    setWinnerLoading(false)
    setShowConfetti(true)

    const currentTime = new Date();
    const nextGameStartTime = new Date(currentTime);
    nextGameStartTime.setSeconds(currentTime.getSeconds() + 10);
    
    let sleepTime = nextGameStartTime - currentTime - 5000;
    if (sleepTime < 0) sleepTime = 0;

    setTimeout(() => {
      let count = 5;
      const intervalId = setInterval(() => {
        setCountdown(count);
        count--;
        if (count < 0) {
          setCanJoin(true)
          setIsActive(true)
          setMadeMove(false)
          clearInterval(intervalId);
          setCountdown("Начинаем!");
          handleJoinGame();
          setWinner('');
          setCountdown(null);
        }
      }, 1000);
    }, sleepTime);
  }

  const handleMakeMove = (move) => {
    if (leaving) return;
    if (!move) {
      toast.info('Выберите одну из карт')
      return;
    }

    setMadeMove(true)
    if (playerOne && playerOne.madeMove ||
        playerTwo && playerTwo.madeMove)
        setWinnerLoading(true)

    setTimeout(() => {
      handleWin({winnerName: 'You'});
    }, 1000);

    service
      .makeMove(move)
      .catch((error) => {
        toast.error(`Ошибка при совершении хода: ${error.message}`);
        setMadeMove(false)
      });
  };

  const handleLeaveGame = () => {
    setLeaving(true)
    service
    .leaveGame()
    .then(() => {
      setIsPlayer(false)
      setCanJoin(true)
    })
    .catch((error) => {
      toast.error(`Ошибка при выходе из игры: ${error.message}`);
    })
    .finally(() => {
      setLeaving(false)
    });
  };

  const handleLeaveRoom = () => {
    setLeaving(true)
    service
      .leaveRoom()
      .then(() => {
      })
      .catch((error) => {
        toast.error(`Ошибка при выходе из комнаты: ${error.message}`);
      })
      .finally(() => {
        setLeaving(false)
        navigate('/')
      });
  };  

  return (
    <div className="game-page">
      {!isActive && (
        <>
        {!countdown && (
          <div className="overlay">
            {showConfetti && (
                <Confetti
                width={width}
                height={height}
                recycle={false}
                numberOfPieces={500}
                confettiSource={{ x: 0, y: 0, w: width, h: height * 0.1 }}
                gravity={0.075}
            />
            )}
            <div className="overlay-content">
              <h1>Игра окончена</h1>
              {winner && <h3>Победил игрок {winner}</h3>}
              {playerOne &&
               playerTwo &&
               playerOne.madeMove &&
               playerTwo.madeMove && 
                <p className="mg-top">
                {playerOne.name} выбрал {playerOne.madeMove}<br/>
                {playerTwo.name} выбрал {playerTwo.madeMove}
                </p>}
            </div>
          </div>
        )}

        {countdown && (
          <div className="overlay">
            <div className="countdown-overlay">
              {typeof countdown === 'number' && (
                <>
                  <div className="countdown-label">До следующей игры осталось</div>
                  <div className="countdown-value">{countdown}</div>
                </>
              )}
              {typeof countdown !== 'number' && (
                <div className="countdown-value">{countdown}</div>
              )}
            </div>
          </div>
        )}
        </>
      )}

      {isActive && (
        <>
          {winnerLoading && (
            <div className="overlay">
              <div className="overlay-content">
                <div className="status-container">
                  <div className="spinner"></div>
                  <p>Идет расчет победителя...</p>
                </div>
              </div>
          </div>
          )}

          <div className="left-side">
            <MovePanel
                myTurn={!playerOne && isPlayer ? {moveMade: madeMove} : null}
                player={playerOne}
                canJoin={canJoin}
                onJoin={handleJoinGame}
                onReady={handleMakeMove}/>
          </div>

          <div className="right-side">
            <MovePanel
                  myTurn={!playerTwo && isPlayer ? {moveMade: madeMove} : null}
                  player={playerTwo}
                  canJoin={canJoin}
                  onJoin={handleJoinGame}
                  onReady={handleMakeMove}/>
          </div>
        </>
      )}
      <div className="center-button">
        <button onClick={isPlayer ? handleLeaveGame : handleLeaveRoom} disabled={leaving}>
          {leaving ? (
            <div className="small-spinner"></div>
          ) : (
            `Покинуть ${isPlayer ? 'игру' : 'комнату'}`
          )}
        </button>
      </div>
    </div>
  );
};

export default GamePage;
