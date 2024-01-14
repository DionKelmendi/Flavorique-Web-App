import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function Follow({ }) {

  return (
    <section className='followSection bg-primary p-5'>
      <div className='m-auto d-flex justify-content-center align-items-center w-60' style={{ color: "white" }}>
        <h3 className="m-0 me-3">Follow us</h3>
        <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
          <i className="bi bi-instagram"></i>
        </Link>
        <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
          <i className="bi bi-pinterest"></i>
        </Link>
        <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
          <i className="bi bi-facebook"></i>
        </Link>
        <Link to="/contact" className='rounded text-primary text-center me-3' style={{ background: "white", width: "40px", fontSize: "27px" }}>
          <i className="bi bi-twitter"></i>
        </Link>
      </div>
    </section>
  )
}