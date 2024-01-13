import { React, useState } from 'react'
import { Link } from 'react-router-dom';
import { useEffect } from 'react';

export default function Header({ userData }) {

  useEffect(() => {
    console.log("Fetching user data");
    console.log(userData);
  }, [])

  return (
    <header>
      <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-primary border-bottom box-shadow mb-3">
        <div className="container-fluid">
          <Link className='navbar-brand text-light' to="/"> Flavorique </Link>
          <button className="navbar-toggler text-light" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
            aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon text-light"></span>
          </button>
          <div className="navbar-collapse collapse d-sm-flex justify-content-between">
            <ul className="navbar-nav justify-content-center w-100">
              <li className="nav-item">
                <Link className='nav-link text-light' to="/"> Home </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/recipes"> Recipes </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/tags"> Tags </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/contact"> Contact </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/about"> About </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/privacy"> Privacy </Link>
              </li>
            </ul>
            <ul className="navbar-nav">
              {userData ? (
                <>
                  <li className="nav-item">
                    <Link className='nav-link text-light' to="/user">
                      {userData.userName || 'Username'}
                    </Link>
                  </li>
                  <li className="nav-item">
                    <a href="https://localhost:7147/Identity/Account/Logout/" className="nav-link btn btn-link text-light">Logout</a>
                  </li>
                </>
              ) : (
                <>
                  <li className="nav-item">
                    <a className="nav-link text-light" asp-area="Identity" asp-page="/Account/Register">Register</a>
                  </li>
                  <li className="nav-item">
                    <a href="https://localhost:7147/Identity/Account/Login/" className="nav-link text-light" asp-area="Identity" asp-page="/Account/Login">Login</a>
                  </li>
                </>
              )}
            </ul>
          </div>
        </div>
      </nav>
    </header>
  )
}