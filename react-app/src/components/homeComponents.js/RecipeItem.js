import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function RecipeItem({ id, date, title, description, src }) {

  return (
    <>
      <div className="border-bottom d-flex align-items-center text-break mb-4 pb-4 hoverable" style={{ height: "175px" }}>
        <img className="h-100" src={src} style={{ aspectRatio: "1 / 1", objectFit: "cover", objectPosition: "center" }} />
        <div className="ps-5 m-0 text-left">
          <p className="text-secondary m-0">{date}</p>
          <h3 className="fw-bolder m-0">{title}</h3>
          <p>{description}</p>
          <Link to={"/Recipes/Detail/" + id} className="text-primary text-decoration-none" style={{ fontSize: "18px", letterSpacing: "1px", margin: 0 }}> Continue Reading <i className="bi bi-arrow-right"></i></Link>
        </div>
      </div>
    </>
  )
}