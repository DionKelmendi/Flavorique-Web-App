import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function CategoryItem({ id, name }) {

  // Get Tags by Category Id, then map them here :)

  return (
    <>
      <h4 className='m-0 mt-5'><b>{name}</b></h4>
      <hr className='mt-2' />
      <ul className='coloredBullets text-primary d-flex justify-content-between flex-wrap' style={{ letterSpacing: "1px", fontSize: "18px" }}>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
        <li className="p-2" style={{ width: "33%" }}><Link to="/" className="text-secondary-emphasis text-decoration-none">Insert Tag Here</Link></li>
      </ul>
    </>
  )
}