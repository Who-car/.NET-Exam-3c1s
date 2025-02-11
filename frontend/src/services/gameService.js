import axiosInstance from './axiosInstance';
import { toast } from 'react-toastify';

export const getListGames = async (pageData) => {
  try {
    const response = await axiosInstance.get('/game/all', { params: pageData });
    return response.data;
  } catch (error) {
    const message = error.response.data.Message || error.message || error
    toast.error(`error [get list games]: ${message}`)
    throw error;
  }
};

export const createGame = async (gameData) => {
    try {
        const response = await axiosInstance.post('/game/create', gameData);
        return response.data;
    } catch (error) {
        const message = error.response.data.Message || error.message || error
        toast.error(`error [create game]: ${message}`)
        throw error;
    }
};

export const getRating = async () => {
    const storedRating = localStorage.getItem('rating');
    if (storedRating) return storedRating;

    try {
        const response = await axiosInstance.get('/game/rating');
        if (response.data) {
            localStorage.setItem('rating', response.data);
          }
        return response.data;
    } catch (error) {
        const message = error.response.data.Message || error.message || error
        toast.error(`error [get rating]: ${message}`)
        localStorage.setItem('rating', "[error]");
        throw error;
    }
  };