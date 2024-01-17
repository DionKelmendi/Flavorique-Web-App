
import logo from './logo.svg';
import './index.css';
import './App.css';
import React, { useState, useEffect } from 'react';
import { Route, Routes, Navigate, useLocation } from "react-router-dom";
import Header from './components/Header';
import Home from './pages/Home';
import Footer from './components/Footer';
import Categories from './pages/Categories';
import Recipes from './pages/Recipes';
import AllRecipes from './pages/AllRecipes';
import DetailRecipe from './pages/DetailRecipe';
import PrintRecipe from './pages/PrintRecipe';
import CreateRecipe from './pages/CreateRecipe';
import Tags from './pages/Tags';
import Dashboard from './pages/Dashboard';


function App() {

  const [userData, setUserData] = useState(null);
  const [error, setError] = useState(null);

  const location = useLocation();

  function ShowHeaderFooter() {
    return true;
  }

  useEffect(() => {
    fetch('https://localhost:7147/api/Account/user', {
      method: 'GET',
      credentials: 'include'
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`Error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        setUserData(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  return (
    <>
      <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css"></link>
      {ShowHeaderFooter() && <Header userData={userData} />}
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/Categories" element={<Categories />} />
        <Route path="/Tags" element={<Tags />} />
        <Route path="/User" element={<Dashboard userData={userData} />} />
        <Route path="/Recipes" element={<Recipes userData={userData} />} />
        <Route path="/Recipes/All" element={<AllRecipes />} />
        <Route path="/Recipes/Create" element={<CreateRecipe />} />
        <Route path="/Recipes/Detail/:id?" element={<DetailRecipe />} />
        <Route path="/Recipes/Print/:id?" element={<PrintRecipe />} />
      </Routes>
      {ShowHeaderFooter() && <Footer userData={userData} />}
    </>
  );
}

export default App;



