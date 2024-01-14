import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function Header({ userData }) {

  useEffect(() => {
    console.log("Fetching user data");
    console.log(userData);
  }, [])

  return (
    <header>
      <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-primary border-bottom box-shadow py-3">
        <div className="container-fluid w-60 p-0">
          <Link to="/" className='navbar-brand text-light' style={{ fontSize: "32px" }}>
            <img height={75} src="https://localhost:7147/logo" className="me-2" style={{ filter: "grayscale(30%)" }} />
            Flavorique
          </Link>
          <button className="navbar-toggler text-light" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
            aria-expanded="false" aria-label="Toggle navigation">
            <span className="navbar-toggler-icon text-light"></span>
          </button>
          <div className="navbar-collapse collapse d-sm-flex justify-content-between align-items-center" style={{ fontSize: "18px" }}>
            <ul className="navbar-nav">
              <li className="nav-item">
                <Link className='nav-link text-light' to="/"> Home </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/Recipes"> Recipes </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/Tags"> Tags </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/Contact"> Contact </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/About"> About </Link>
              </li>
              <li className="nav-item">
                <Link className='nav-link text-light' to="/Privacy"> Privacy </Link>
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