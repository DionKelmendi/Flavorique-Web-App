import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';

export default function SmallTagItem({ id, name, src }) {

  return (
    <Link to={"Tag/" + id} className='text-decoration-none hoverable'>
      <div className='text-center'>
        <img className='border rounded-circle' style={{ height: "100px", aspectRatio: "1/1", objectFit: "cover", objectPosition: "center" }} src={src}></img>
        <p className='text-center' style={{ width: "110px", color: "black", lineHeight: "20px" }}><b>{name}</b></p>
      </div>
    </Link>
  )
}