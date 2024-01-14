
import logo from './logo.svg';
import './index.css';
import React, { useState, useEffect } from 'react';
import { Route, Routes, Navigate, } from "react-router-dom";
import Header from './components/Header';
import Home from './pages/Home';
import Footer from './components/Footer';
import Categories from './pages/Categories';
import Recipes from './pages/Recipes';
import AllRecipes from './pages/AllRecipes';
import Tags from './pages/Tags';


function App() {

  const [userData, setUserData] = useState(null);
  const [error, setError] = useState(null);

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
        console.log("Setting user data");
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
      <Header userData={userData} />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/Categories" element={<Categories />} />
        <Route path="/Tags" element={<Tags />} />
        <Route path="/Recipes" element={<Recipes />} />
        <Route path="/Recipes/All" element={<AllRecipes />} />
      </Routes>
      <Footer userData={userData} />
    </>
  );
}

export default App;



