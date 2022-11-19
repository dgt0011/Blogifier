document.addEventListener("DOMContentLoaded", function () {

  const categories = document.getElementsByClassName("sub-category");
  for (let i = 0; i < categories.length; i++) {
    categories[i].addEventListener('shown.bs.collapse', function (event) {
      console.log('expanded ' + event.target.id);
      saveExpandedCategory("category", event);
    });
    categories[i].addEventListener('hidden.bs.collapse', function (event) {
      console.log('collapsed ' + event.target.id);
      saveCollapsedCategory("category", event);
    });

    restoreCategoryState(categories[i]);
  }
});

function saveExpandedCategory(storageKey, e) {
  localStorage.setItem(e.target.id, e.target.id);
};

function saveCollapsedCategory(storageKey, e) {
  localStorage.setItem(e.target.id, 0);
};

function restoreCategoryState(element) {
  var activeItem = localStorage.getItem(element.id);
  if (activeItem) {
    if (activeItem === "0") {
      element.classList.removeClass('show');
    } else {
      element.classList.add('show');

    }
  }
};
