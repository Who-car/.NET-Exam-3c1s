import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { toast } from 'react-toastify';
import signalRService from './../../services/signalrService';
import rockImg from './../../assets/icons8-coal-64(1).png';
import paperImg from './../../assets/icons8-paper-64(1).png';
import scissorsImg from './../../assets/icons8-scissors-64 (1).png';
import './GamePage.css';

const GamePage = () => {
  const { roomId } = useParams();
  const [isPlayer, setIsPlayer] = useState(false);
  const [isActive, setIsActive] = useState(false); // если хотя бы один из игроков не пустой
  const [canJoin, setCanJoin] = useState(false);
  const [playerOne, setPlayerOne] = useState(null); // {name, isWinner, madeMove}
  const [playerTwo, setPlayerTwo] = useState(null); // {name, isWinner, madeMove}
  const [winner, setWinner] = useState(null);
  const [selectedCard, setSelectedCard] = useState(null);

  useEffect(() => {
    // подключение не к руме, а к игре!
    // к руме подключаются спектаторы, к игре - игроки
    signalRService.on('OnUserConnected', (data) => {
      if (!data.gameStarted && data.playerName) 
        setPlayerOne(prev => ({ ...prev, name: data.playerName }));
      if (data.gameStarted && data.playerName) 
        setPlayerTwo(prev => ({ ...prev, name: data.playerName }));

      if (data.gameStarted) {
        toast.success(`Игрок ${data.playerName} подключился. Игра началась!`)
      } else {
        toast.info(`Игрок ${data.playerName} подключился. Ждем подключения второго игрока`);
      }
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
      if (playerOne.name === data.playerName) 
        setPlayerOne(prev => ({ ...prev, madeMove: true }));
      if (playerTwo.name === data.playerName) 
        setPlayerTwo(prev => ({ ...prev, madeMove: true }));
    });

    signalRService.on('OnWinnerCalculated', (data) => {
      setIsActive(false)
      setWinner(data.winnerName)
      //TODO:
      // 1. Хлопушки победителю в течении 5 секунд
      // 2. Посчитать data.nextGameStartTime - current time - 5 секунд = sleep time
      // 3. После sleep time показывать каждую секунду обратный счет
      // 4. После окончания отсчета обновить canJoin и вызвать JoinGame
    });

    signalRService
      .joinRoom(roomId)
      .then((result) => {
        setCanJoin(result.joinGame);
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

    return () => {
      signalRService.off('OnUserConnected');
      signalRService.off('OnUserDisconnected');
      signalRService.off('OnMoveMade');
      signalRService.off('OnWinnerCalculated');
    };
  }, [roomId]);

  const handleCardClick = (card) => {
    setSelectedCard(card);
  };

  const handleJoinGame = () => {
    if (!canJoin) toast.error('Игра уже началась')
      
    signalRService
      .joinGame(roomId)
      .then((result) => {
        if (result.isSuccess) {
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

  const handleMakeMove = (move) => {
    signalRService
      .makeMove(move)
      .catch((error) => {
        toast.error(`Ошибка при совершении хода: ${error.message}`);
      });
  };

  const handleLeaveGame = () => {
    signalRService.leaveGame().catch((error) => {
      toast.error(`Ошибка при выходе из игры: ${error.message}`);
    });
  };

  const handleLeaveRoom = () => {
    signalRService.leaveRoom().catch((error) => {
      toast.error(`Ошибка при выходе из комнаты: ${error.message}`);
    });
  };

  //TODO: если isActive = false, то показывать серый экран "Игра окончена. Выиграл {winner}". А ИНАЧЕ >>>>
  //TODO: если canJoin = false и isPlayer = false, то показывать 
  //      - у того игрока у которого player.madeMove = false - "Игрок думает..."
  //      - у того игрока у которого player.madeMove = true - "Игрок сделал ход"
  //TODO: если canJoin = false и isPlayer = true, то показывать
  //      - все три карточки у себя
  //      - "Игрок думает..." - если player.madeMove = false
  //      - "Игрок сделал ход..." - если player.madeMove = true
  //TODO: если canJoin = true, то показывать посередине мелькающую кнопку "Присоединиться" 

  return (
    <div className="game-page">
      {/* Левая часть */}
      <div className="left-side">
        <h2>Ваш Ход</h2>
        <div className="cards-container">
          <div
            className={`card ${selectedCard === 'rock' ? 'selected' : ''}`}
            onClick={() => handleCardClick('rock')}
          >
            <img src={rockImg} alt="Камень" width="128" height="128" />
            <p>Камень</p>
          </div>
          <div
            className={`card ${selectedCard === 'paper' ? 'selected' : ''}`}
            onClick={() => handleCardClick('paper')}
          >
            <img src={paperImg} alt="Бумага" width="128" height="128" />
            <p>Бумага</p>
          </div>
          <div
            className={`card ${selectedCard === 'scissors' ? 'selected' : ''}`}
            onClick={() => handleCardClick('scissors')}
          >
            <img src={scissorsImg} alt="Ножницы" width="128" height="128" />
            <p>Ножницы</p>
          </div>
        </div>
        <button className="ready-button">Готов!</button>
      </div>

      {/* Правая часть */}
      <div className="right-side">
        <div className="thinking-content">
          <div className="spinner"></div>
          <p>Игрок думает...</p>
        </div>
      </div>
    </div>
  );
};

export default GamePage;
