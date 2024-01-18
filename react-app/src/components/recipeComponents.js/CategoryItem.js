import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function CategoryItem({ id, category, tags }) {

  const liTags = tags.map(tag => (
    <li key={tag.id} className="p-2" style={{ width: "33%" }}><Link to={"/Tag/" + tag.id} className="text-secondary-emphasis text-decoration-none listHoverable">{tag.name}</Link></li>
  ));

  return (
    <>

      <h4 className='m-0 mt-5'><b><Link to={"/Category/" + category.id} className='text-decoration-none'> {category.name} </Link></b></h4>
      <hr className='mt-2' />
      <ul className='coloredBullets text-primary d-flex justify-content-between flex-wrap' style={{ letterSpacing: "1px", fontSize: "18px" }}>
        {liTags}
      </ul>
    </>
  )
}