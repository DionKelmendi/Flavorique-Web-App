
import logo from './logo.svg';
import './index.css';
import './App.css';
import React, { useState, useEffect } from 'react';
import { Route, Routes, Navigate, useLocation } from "react-router-dom";
import Header from './components/Header';
import Footer from './components/Footer';
import Recipes from './pages/Recipes';
import AllRecipes from './pages/AllRecipes';
import DetailRecipe from './pages/DetailRecipe';
import PrintRecipe from './pages/PrintRecipe';
import CreateRecipe from './pages/CreateRecipe';
import Tag from './pages/Tag';
import Tags from './pages/Tags';
import Dashboard from './pages/Dashboard';
import Home from './pages/Home';
import EditRecipe from './pages/EditRecipe';
import Category from './pages/Category';

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
        <Route path="/Category/:id?" element={<Category />} />
        <Route path="/Tag/:id?" element={<Tag />} />
        <Route path="/Tags" element={<Tags />} />
        <Route path="/User" element={<Dashboard userData={userData} />} />
        <Route path="/Recipes" element={<Recipes userData={userData} />} />
        <Route path="/Recipes/All" element={<AllRecipes />} />
        <Route path="/Recipes/Create" element={<CreateRecipe />} />
        <Route path="/Recipes/Edit/:id?" element={<EditRecipe />} />
        <Route path="/Recipes/Detail/:id?" element={<DetailRecipe userData={userData} />} />
        <Route path="/Recipes/Print/:id?" element={<PrintRecipe />} />
      </Routes>
      {ShowHeaderFooter() && <Footer userData={userData} />}
    </>
  );
}

export default App;



