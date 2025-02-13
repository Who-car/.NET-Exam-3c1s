import React, { useState } from 'react';
import './MovePanel.css';
import rockImg from './../../assets/icons8-coal-64(1).png';
import paperImg from './../../assets/icons8-paper-64(1).png';
import scissorsImg from './../../assets/icons8-scissors-64 (1).png';

const MovePanel = ({ myTurn, player, canJoin, onJoin, onReady, onLeaveGame, onLeaveRoom }) => {
    const [selectedCard, setSelectedCard] = useState(null);

    const handleCardClick = (card) => {
        setSelectedCard(card);
      };

    const handleJoin = () => {
        onJoin();
    }

    const handleReady = () => {
        onReady(selectedCard);
    }

    const handleLeaveGame = () => {
        onLeaveGame();
    }

    const handleLeaveRoom = () => {
        onLeaveRoom();
    }

    return (
    <div className="panel-container">
        {!myTurn && (
        <>
            {(!player || !player.name) && (
                <div className="status-container">
                    <div className="spinner"></div>
                    <p>Ожидаем подключения</p>
                    {canJoin && (
                        <button className="join-button pulsate" onClick={handleJoin}>
                        Присоединиться
                        </button>
                    )}
                </div>
            )}

            {(player && player.name) && (
                <>
                    {!player.madeMove ? (
                    <div className="status-container">
                        <div className="spinner"></div>
                        <p>{player.name} делает ход...</p>
                    </div>
                    ) : (
                    <div className="status-container">
                        <p className="spinner-mg">{player.name} сделал свой ход.</p>
                    </div>
                    )}
                </>
            )}
        </>
        )}
        
        {myTurn && !myTurn.moveMade && (
            <>
                <h2>Ваш Ход</h2>
                <div className="cards-container">

                    <div
                    className={`card ${selectedCard === 'Rock' ? 'selected' : ''}`}
                    onClick={() => handleCardClick('Rock')}>
                        <img src={rockImg} alt="Камень" width="128" height="128" />
                        <p>Камень</p>
                    </div>

                    <div
                    className={`card ${selectedCard === 'Paper' ? 'selected' : ''}`}
                    onClick={() => handleCardClick('Paper')}>
                        <img src={paperImg} alt="Бумага" width="128" height="128" />
                        <p>Бумага</p>
                    </div>

                    <div
                    className={`card ${selectedCard === 'Scissors' ? 'selected' : ''}`}
                    onClick={() => handleCardClick('Scissors')}>
                        <img src={scissorsImg} alt="Ножницы" width="128" height="128" />
                        <p>Ножницы</p>

                    </div>
                </div>
                <button className="ready-button" onClick={handleReady}>Готов!</button>
            </>
        )}

        {myTurn && myTurn.moveMade && (
            <div className="status-container">
                <p className="spinner-mg">Вы сделали свой ход.</p>
            </div>
        )}
    </div>
  );
};

export default MovePanel;