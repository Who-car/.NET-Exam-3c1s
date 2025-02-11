import axiosInstance from './axiosInstance';
import { toast } from 'react-toastify';

export const getListGames = async (pageData) => {
  try {
    const response = await axiosInstance.get('/game/all', { params: pageData });
    return response.data;
  } catch (error) {
    toast.error(`error [get list games]: ${error.message || error}`)
    throw error;
  }
};

export const createGame = async (gameData) => {
    try {
        const response = await axiosInstance.post('/game/create', gameData);
        return response.data;
    } catch (error) {
        toast.error(`error [create game]: ${error.message || error}`)
        throw error;
    }
};

export const getRating = async () => {
    const storedRating = localStorage.getItem('rating');
    if (storedRating) return storedRating;

    try {
        const response = await axiosInstance.get('/game/rating');
        if (response.data.rating) {
            localStorage.setItem('rating', response.data.rating);
          }
        return response.data.rating;
    } catch (error) {
        toast.error(`error [get rating]: ${error.message || error}`)
        localStorage.setItem('rating', "[error]");
        throw error;
    }
  };