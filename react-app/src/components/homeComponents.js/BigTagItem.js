import { React, useState } from 'react'
import { Link } from 'react-router-dom';
import { useEffect } from 'react';

export default function BigTagItem({ id, name, src }) {

  return (
    <Link to={"tags/" + id} className='text-decoration-none'>
      <div className='text-center'>
        <img className='object-fit-cover' style={{ height: "400px", aspectRatio: "11/16" }} src={src}></img>
        <p className='bg-warning p-2 w-50 m-auto' style={{ transform: "translateY(-20px)", color: "white", letterSpacing: "1px" }}><b>{name}</b></p>
      </div>
    </Link>
  )
}