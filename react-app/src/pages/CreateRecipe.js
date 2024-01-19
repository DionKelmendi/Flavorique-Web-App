import React, { useState, useEffect, useRef } from 'react';
import { CKEditor } from '@ckeditor/ckeditor5-react';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { Link, useNavigate } from 'react-router-dom';

export default function CreateRecipe() {
  const [bodyData, setBodyData] = useState('');
  const [titleData, setTitleData] = useState('');
  const [recipeId, setRecipeId] = useState(0);
  const [error, setError] = useState([]);
  const [tagData, setTagData] = useState([]);
  const [tagString, setTagString] = useState("");
  const tagSelect = useRef(null);
  const selectedTagsDiv = useRef(null);
  const selectedTagsInput = useRef(null);
  const navigate = useNavigate();

  // Tag Handler Functions

  useEffect(() => {
    tagSelect.current = document.getElementById('tagSelect');
    selectedTagsDiv.current = document.getElementById('selectedTags');
    selectedTagsInput.current = document.getElementById('selectedTagsInput');

    initializeSpans();
  }, [])

  useEffect(() => {
    fetch('https://localhost:7147/api/Category/tags', {
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
        setTagData(data);
      })
      .catch(error => {
        setError(error.message);
        console.log(error);
      });
  }, []);

  const tagOptions = tagData.map(data => (
    <optgroup key={data.category.id} label={data.category.name}>
      {data.tags.map(tag => (
        <option key={tag.id} value={tag.id}>{tag.name}</option>
      ))}
    </optgroup>
  ));

  function createTagSpan(value, text) {
    const tagSpan = document.createElement('span');
    tagSpan.classList.add('btn', 'btn-primary', 'me-2');
    tagSpan.textContent = text;
    tagSpan.setAttribute('data-value', value);
    tagSpan.addEventListener('click', () => removeTag(tagSpan));

    return tagSpan;
  }

  function initializeSpans() {
    const selectedTagsDiv = document.getElementById('selectedTags');
    const selectedTagsInput = document.getElementById('selectedTagsInput');

    const inputValues = selectedTagsInput.value.trim();

    if (inputValues !== '') {
      const valuesArray = inputValues.split(',').map(value => value.trim());

      valuesArray.forEach(value => {

        const option = Array.from(tagSelect.options).find(option => option.value === value);
        const tagSpan = createTagSpan(value, option.text);
        selectedTagsDiv.appendChild(tagSpan);
      });
    }
  }

  function addTag() {
    Array.from(tagSelect.current.selectedOptions).forEach(option => {
      if (!tagExists(selectedTagsDiv.current, option.value)) {
        const tagSpan = createTagSpan(option.value, option.text);
        selectedTagsDiv.current.appendChild(tagSpan);
      }
    });

    tagSelect.current.selectedIndex = 0;
    updateSelectedTagsInput();
  }

  function tagExists(selectedTagsDiv, value) {
    return Array.from(selectedTagsDiv.children).some(span => span.getAttribute('data-value') === value);
  }

  function removeTag(tagSpan) {
    selectedTagsDiv.current.removeChild(tagSpan);
    updateSelectedTagsInput();
  }

  function updateSelectedTagsInput() {
    const valuesArray = Array.from(selectedTagsDiv.current.children).map(span => span.getAttribute('data-value'));
    selectedTagsInput.current.value = valuesArray.join(', ');
    setTagString(selectedTagsInput.current.value);
  }

  // Create Recipe Functions

  useEffect(() => {
    ClassicEditor
      .create(document.querySelector('#editor'), {
        ckbox: {
          tokenUrl: "https://localhost:7147/api/CKBox/token",
          theme: 'lark'
        },
        removePlugins: ['FormatPainter', 'Table'],
      })
      .then(editor => {
        editor.model.document.on('change:data', () => {
          handleEditorChange(null, editor);
        });
      })
      .catch(error => {
        console.log(error);
      });
  }, []);

  const handleEditorChange = (event, editor) => {
    const data = editor.getData();
    setBodyData(data);
  };

  const handleTitleChange = (e) => {
    setTitleData(e.target.value);
  };

  const handleFormSubmit = (e) => {
    e.preventDefault();

    const currentDate = new Date();

    const recipeJsonData = {
      "id": 0,
      "title": titleData,
      "body": bodyData,
      "createdDateTime": currentDate.toISOString(),
      "authorId": null,
      "comments": null,
      "author": null
    };

    fetch("https://localhost:7147/api/Recipe", {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
      body: JSON.stringify(recipeJsonData),
    })
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        setRecipeId(data.id);

        const recipeTagJsonData = {
          "recipeId": data.id,
          "tagIds": tagString
        };

        fetch("https://localhost:7147/api/Recipe/RecipeTag", {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          credentials: 'include',
          body: JSON.stringify(recipeTagJsonData),
        })
          .then(response => {
            if (!response.ok) {
              throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
          })
          .then(data => {
            console.log('Success:', data);
          })
          .catch(error => {
            setError(error.message);
          });

        navigate('/Recipes/Detail/' + data.id);
      })
      .catch(error => {
        console.error('Error:', error);
        setError(error.message);
      });
  };

  return (
    <section className='createRecipeContainer'>
      <section>
        <section style={{ height: "90px" }}></section>
        <section className='heroContainer' style={{ background: "#f7f7f7" }}>
          <div className="m-auto text-center p-5 pb-3 w-60">
            <h1 className="mb-5">Publish your recipe</h1>
            <h5 className='text-secondary w-75 m-auto text-justify'>Craft your culinary masterpieces with ease and creativity. Unleash your inner chef, and with every click and creation, discover the joy of crafting delicious moments in the kitchen. We thank you for opening up a window to your soul via the sharing of this recipe.</h5>
            <div className='mt-5'></div>
            <div className="form-group">
              <div className="input-group">
              </div>
            </div>
          </div>
        </section>

        <div className='w-60 m-auto pt-5 overflow-hidden'>
          <div className='rotator d-flex overflow-hidden w-100' style={{ transition: "0.3s all ease-in" }}>
            <div className='rotatingItem w-100'>
              <div>
                <input type="text" placeholder='Title' className="form-control rounded-0 me-2 mb-2" value={titleData} onChange={handleTitleChange} />
              </div>
              <div id="editor" />
            </div>
          </div>

          <div className="mb-2 mt-3">
            <label>Select Tags</label>
            <select onChange={addTag} id="tagSelect" className="form-control">
              <option value="">Select a Tag</option>
              {tagOptions}
            </select>

            <div className="mt-3">
              <label>Selected Tags:</label>
              <div className="mt-2" id="selectedTags"></div>
              <input type="hidden" id="selectedTagsInput" name="SelectedTags" />
            </div>
          </div>

          <form onSubmit={handleFormSubmit}>
            <button type='submit' className={`btn btn-primary`} style={{ float: "right" }}>
              Submit Recipe <i className="bi bi-plus-square"></i>
            </button>
          </form>
        </div>
      </section>
    </section>
  );
}
