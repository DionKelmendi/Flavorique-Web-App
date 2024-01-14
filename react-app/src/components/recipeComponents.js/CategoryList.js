import { React, useState, useEffect } from 'react'
import { Link } from 'react-router-dom';
import CategoryItem from './CategoryItem';

export default function CategoryList({ amount }) {

  const categoryItems = Array(amount).fill(null).map((_, index) => (
    <CategoryItem id="3" name="Popular Categories" />
  ));

  return (
    <>
      <section className="pt-4 pb-5" style={{ background: "#f7f7f7" }}>
        <div className='m-auto w-60'>
          {categoryItems}
        </div>
      </section>
    </>
  )
}