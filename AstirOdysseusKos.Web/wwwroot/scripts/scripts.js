/*"use strict";

var app = function () {
  var body = undefined;
  var menu = undefined;
  var menuItems = undefined;
  var navContainer = undefined;
  var mainMenuSecLevel = undefined;
  var elLinks = undefined;

  var init = function () {
    body = document.querySelector('body');
    menu = document.querySelector('.menu-icon');
    navContainer = document.querySelector(".nav__content");
    menuItems = document.querySelectorAll('.nav__list-item');
    mainMenuSecLevel = document.querySelector(".menu__list--level-1");
    elLinks = document.querySelectorAll(".nav__list-item .menu__link");
    
    const elArrows = document.querySelectorAll(".nav__list-item .arrow");
    applyListeners();
    
    if (elArrows.length) {
        elArrows.forEach(e => e.addEventListener('click', (t) => {
            const eParent = e.closest('.menu__item--has-children');
    
            if (eParent.classList.contains('toggled')) {
                eParent.classList.toggle("toggled");
                navContainer.classList.remove('tabbed-mob');
            }else{
                i.forEach((element) => {
                  element.classList.remove('toggled');
                });
                eParent.classList.add("toggled");
                navContainer.classList.add('tabbed-mob');
            }
        }));
    }
        
  };

  var applyListeners = function () {
    menu.addEventListener('click', function () {
      return toggleClass(body, 'nav-active');
    });
  };

  var toggleClass = function (element, stringClass) {
    if (element.classList.contains(stringClass))
      element.classList.remove(stringClass);
    else
      element.classList.add(stringClass);
  };

  init();
}();*/

document.addEventListener('DOMContentLoaded', function () {
    
    
    /* NAVIGATION
    ================================== */
    var el_menuBtn = document.querySelector('.menu-trigger');
    var el_BodyHtml = document.querySelector('html');
    var el_menuOverlay = document.querySelector('.main_navigation');
    if (el_menuBtn !== null) {
        el_menuBtn.onclick = function() {

            el_menuBtn.classList.toggle('is-clicked');
            el_BodyHtml.classList.toggle('main-navigation-visible');
            
            if (window.matchMedia('(max-width: 767px)').matches) {
               el_BodyHtml.classList.toggle('mobile-device'); 
            }
            if (el_menuBtn.classList.contains("active")) {
               // el_menuOverlay.style.height = "100vh";
                el_menuOverlay.classList.add('opened');
            }else{
                el_menuOverlay.classList.remove('opened');
                //document.querySelector('header').style.transitionDuration = "1s";
            }
        }
    }
    
    document.querySelectorAll("li.parent").forEach(theLi => {
      theLi.addEventListener("mouseover", function () {
           theLi.classList.add('expanded');
      });
      theLi.addEventListener("mouseout", function () {
          theLi.classList.remove('expanded');
      });
    });



    const myCarouselElement = document.querySelector('#bigCarousel');
    if(myCarouselElement){
        const carousel = new bootstrap.Carousel(myCarouselElement, {
          interval: 3000,
          pause: false
        });
    }
    
    const scrollBtn = document.querySelector(".scrollup");
    window.addEventListener('scroll', function() {
        let scroll = window.scrollY;
        let el_header = document.querySelector('header');
        let el_Body = document.querySelector('body');
        if (scroll > 50) {
            el_header.classList.add('headerBg');
            el_Body.classList.add('scrolled');
        } else {
            el_header.classList.remove('headerBg');
            el_Body.classList.remove('scrolled');
        }
        
        if(scrollBtn){
            if (scroll > 100) {
                scrollBtn.classList.add('is-visible');
            } else {
               scrollBtn.classList.remove('is-visible');
            }
        }
    });
    
   /* MODALS
    ================================== */
    var myModalElement = document.getElementById('popupModal'); // relatedTarget
    if (myModalElement) {
        setTimeout(function () {
            let modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('popupModal')); // Returns a Bootstrap modal instance
            modal.show();
        }, 2000);  
    }   
    
      /* DATE PICKER -flatpickr  okkkkkkkkkkkkkkkkkkkkkkkk
    ====================================================== */
   
   
  /*  $("select[name=bookingLinkSel]").change(function () {
		var url=$(this).closest("form").attr("action", $(this).val());
	 });
	 */
	 document.querySelector('select[name=bookingLinkSel]').addEventListener('change',function(){
	     this.closest('form').setAttribute("action", this.value);
       // document.querySelector('#UserName').value = this.value;
    });
	 
    var checkinDate=document.getElementById("checkin");
    var checkoutDate=document.getElementById("checkout");
    var currentDate=new Date();
    var openingDate=new Date("04/20/2024"); // m/d/yy
    if (openingDate > currentDate) {
        currentDate=openingDate;
    }
    if(checkinDate || checkoutDate){
        var checkinPicker= flatpickr(checkinDate, {
            dateFormat: 'd/m/Y',
            defaultDate:currentDate,
            minDate:"today",
            onChange: function(selectedDates, dateStr, instance) {
                checkoutDate.flatpickr({ 
                    dateFormat: 'd/m/Y',
                    minDate: dateStr
                });
            },
        });
        
        var checkoutPicker= flatpickr(checkoutDate, {
            dateFormat: 'd/m/Y',
            defaultDate: currentDate.setDate(currentDate.getDate() + 5)
        });
    }  
    
    
    var inputArrivalDate=document.getElementById("ArrivalDate");
    var inputDepartureDate=document.getElementById("DepartureDate");

    if(inputArrivalDate || inputDepartureDate){
        var endPicker= flatpickr(inputDepartureDate, {
            dateFormat: 'd/m/Y'
        });
        var startPicker= flatpickr(inputArrivalDate, {
            dateFormat: 'd/m/Y',
            minDate:"today",
            onChange: function(selectedDates, dateStr, instance) {
                inputDepartureDate.flatpickr({ 
                    dateFormat: 'd/m/Y',
                    minDate: dateStr
                });
            },
        });
    }  
    
    var inputTimeArrival=document.getElementById("ArrivalTime");
    if(inputTimeArrival){
         var inputTimePicker= flatpickr(inputTimeArrival, {
             enableTime: true,
            noCalendar: true,
             time_24hr: true,
            dateFormat: "H:i",
         });
    }
    var inputTimeDeparture=document.getElementById("DepartureTime");
    if(inputTimeDeparture){
         var inputTimePicker= flatpickr(inputTimeDeparture, {
             enableTime: true,
            noCalendar: true,
             time_24hr: true,
            dateFormat: "H:i",
         });
    }
    
    
// SWIPER CAROUSELS
// ==============================
    
    var swiperIntroRooms = new Swiper(".roomsSwiper", {
      slidesPerView: 1,
      spaceBetween: 0,
        loop:true,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      breakpoints: {
        768: {
          slidesPerView: 1.3,
          spaceBetween: 15,
        },
        992: {
          slidesPerView: 2.5,
          spaceBetween: 20,
        },
       /* 1199: {
          slidesPerView: 2.7,
          spaceBetween: 20,
        },*/
      },
    });
    
    var swiperBoxes = new Swiper(".boxesSwiper", {
      slidesPerView: 1,
      spaceBetween: 0,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      breakpoints: {
        768: {
          slidesPerView: 1.3,
          spaceBetween: 15,
        },
        992: {
          slidesPerView: 1.2,
          spaceBetween: 20,
        },
        1200: {
          slidesPerView: 1.7,
          spaceBetween: 20,
        }
      },
    });
    
    var swiperBlog = new Swiper(".blogCarousel", {
      loop: false,
        slidesPerView: 'auto',
        spaceBetween:20,
        freeMode: true,
        breakpoints: {
            0: {
                slidesPerView: 1.2,
            },
            768: {
                slidesPerView: 2.2,
            },
            990: {
                slidesPerView: 2.2,
            },
            1200: {
                slidesPerView: 'auto',
            }
        },
    });
    
    var swiperReviews = new Swiper(".testimonialsSwiper", {
      slidesPerView: 1,
      spaceBetween: 0,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      }
    });

    var swiperAwards = new Swiper(".awardsSwiper", {
      slidesPerView: 2,
      spaceBetween: 0,
      /*pagination: {
        el: ".swiper-pagination",
        clickable: true,
      },*/
      navigation: {
        nextEl: ".awards-section .swiper-button-next",
        prevEl: ".awards-section .swiper-button-prev",
      },
      breakpoints: {
        768: {
          slidesPerView: 3,
          spaceBetween: 10
        }
      },
      breakpoints: {
        992: {
          slidesPerView: 5,
          spaceBetween: 10
        }
      },
    });
    
    











    var swiperExperiences = new Swiper(".expSwiper", {
      slidesPerView: 1,
      spaceBetween: 0,
      autoHeight: true, 
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      breakpoints: {
        576: {
          slidesPerView: 1.5,
          spaceBetween: 15,
        },
        768: {
          slidesPerView: 1.3,
          spaceBetween: 15,
        },
        992: {
          slidesPerView: 2.3,
          //centeredSlides: true,
          spaceBetween: 20,
        },
        1200: {
          slidesPerView: 2.3,
          //centeredSlides: true,
          spaceBetween: 20,
        }
      },
    });
    
    
    var swiperOtherBoxes = new Swiper(".othersSwiper", {
      slidesPerView: 1,
      spaceBetween: 0,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      breakpoints: {
        768: {
          slidesPerView: 2,
          spaceBetween: 15,
        },
        1200: {
          slidesPerView: 3,
          spaceBetween: 20,
        },
      },
    }); 
    
    var swiperGal = new Swiper(".simpleGallerySwiper", {
      lazy: true,
      slidesPerView: 1,
      spaceBetween: 0,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
        autoplay: {
          delay: 5000,
          waitForTransition: true    
      }
    });
    
    var swiperRoomGal = new Swiper(".roomGallerySwiper", {
      lazy: true,
      slidesPerView: 1,
      spaceBetween: 0,
        loop:true,
      navigation: {
        nextEl: ".swiper-button-next",
        prevEl: ".swiper-button-prev",
      },
      breakpoints: {
        768: {
          slidesPerView: 1.3,
          spaceBetween: 15,
        },
        992: {
          slidesPerView: 1.5,
          spaceBetween: 20,
        }
      }
    });    
    
    

    
      /*FILTERS
  ===============================*/  
  
 /* all-rooms*/
  
    const filtersDropMobile = document.querySelector('.filters-drop');
    if (filtersDropMobile){
    filtersDropMobile.addEventListener('click', function() {
        if (this.parentNode.classList.contains('open')) {
            this.parentNode.classList.remove('open');
        }else{
            this.parentNode.classList.add('open');
        }
    });
    }
  
    const filters = document.querySelectorAll('.filter');
    const filtersActiveOnLoad = document.querySelector(".filter.active");


    filters.forEach(filter => { 
      filter.addEventListener('click', function() {
          

        let selectedFilter = filter.getAttribute('data-filter');
        //let itemsToHide = document.querySelectorAll(`.all-rooms .room-box:not([data-filter='${selectedFilter}'])`);
        //let itemsToShow = document.querySelectorAll(`.all-rooms [data-filter='${selectedFilter}']`);
        let itemsToHide = document.querySelectorAll(`.all-rooms .room-box:not([data-filter*='${selectedFilter}'])`);
        let itemsToShow = document.querySelectorAll(`.all-rooms [data-filter*='${selectedFilter}']`);
        
        if (selectedFilter == 'all') {
          itemsToHide = [];
          itemsToShow = document.querySelectorAll('.all-rooms [data-filter]');
        }
        

        filters.forEach(el => {
          el.classList.remove('active');
        });
        filter.classList.add('active'); 
    
        itemsToHide.forEach(el => {
          el.classList.add('hide');
          el.classList.remove('show');
        });
    
        itemsToShow.forEach(el => {
          el.classList.remove('hide');
          el.classList.add('show'); 
        });
    
      });
    });
    
    if (filtersActiveOnLoad) {
        filtersActiveOnLoad.click();
    }
    
   /*END FILTERS
  ===============================*/
  
  
   /* FORMS
    ================================== */
    var formSubmitted = document.getElementById("formResult");
    if (formSubmitted) 
    {
     
        var offset = formSubmitted.offsetTop;
           //alert(offset);
        window.scrollTo({ top: offset-30, behavior: 'smooth'});
    }
    
    /* FORMS VAIDATION
    ==================================  */
       
    window.valid = false;
    const handlers = window["unobtrusive-validation"].validationHandlers;
    handlers.push(function(evt, succeeded) {
        if (succeeded) {
            window.valid = true;  
            console.log('Validation Completed');

            document.addEventListener('submit', (e) => {
                // Store reference to form to make later code easier to read
                const form = e.target;

                // get status message references
                const statusBusy = form.querySelector('.status-busy');
                const statusFailure = form.querySelector('.status-failure');

                // Post data using the Fetch API
                fetch(form.action, {
                    method: form.method,
                    body: new FormData(form),
                })
                    // We turn the response into text as we expect HTML
                    .then((res) => res.text())

                    // Let's turn it into an HTML document
                    .then((text) => new DOMParser().parseFromString(text, 'text/html'))

                    // Now we have a document to work with let's replace the <form>
                    .then((doc) => {
                         
                        // Create result message container and copy HTML from doc
                        const result = document.createElement('div');
                        result.innerHTML = doc.body.innerHTML;

                        // Allow focussing this element with JavaScript
                        result.tabIndex = -1;

                        // And replace the form with the response children
                        form.parentNode.replaceChild(result, form);

                        // Move focus to the status message
                        result.focus();
                    })
                    .catch((err) => {
                        // Unlock form elements
                        Array.from(form.elements).forEach(
                            (field) => (field.disabled = false)
                        );

                        // Return focus to active element
                        lastActive.focus();

                        // Hide the busy state
                        statusBusy.hidden = false;

                        // Show error message
                        statusFailure.hidden = false;
                    });

                // Before we disable all the fields, remember the last active field
                const lastActive = document.activeElement;

                // Show busy state and move focus to it
                statusBusy.hidden = false;
                statusBusy.tabIndex = -1;
                statusBusy.focus();

                // Disable all form elements to prevent further input
                Array.from(form.elements).forEach((field) => (field.disabled = true));

                // Make sure connection failure message is hidden
                statusFailure.hidden = true;

                // Prevent the default form submit
                e.preventDefault();
            });
            //document.querySelector(".validateForm").submit();   
        }else{
            evt.preventDefault(); 
        }
    });
   
    
    
    var inputAttachment = document.getElementById("attachment");
    if (inputAttachment) {
        inputAttachment.addEventListener('change', function (e) {

            //Maximum allowed size in bytes 5MB Example
            const maxAllowedSize = 5 * 1024 * 1024;   //((file[0].size/1024)/1024).toFixed(4); 
            let file = e.currentTarget.files; // puts all files into an array
            let filesize = file[0].size;
            
            if (filesize > maxAllowedSize) {
                this.classList.remove("input-validation-error");
                document.getElementById("approvedFiles").innerText = "File must be smaller than 5MB.";
                document.getElementById("approvedFiles").classList.remove("valid");

                inputAttachment.value = '';
            }else {
                this.classList.remove("input-validation-error");
                document.querySelector(".upload-valid-js").innerText="";
                document.getElementById("approvedFiles").innerText="";
                document.getElementById("approvedFiles").classList.add("valid");
            }
        });
    }  


}, false);       


 // FOR SCROLL TO TOP
let calcScrollValue = () => {
  let scrollProgress = document.getElementById("progress");
  let progressValue = document.getElementById("progress-value");
  let pos = document.documentElement.scrollTop;
  let calcHeight =
    document.documentElement.scrollHeight -
    document.documentElement.clientHeight;
  let scrollValue = Math.round((pos * 100) / calcHeight);
  if (pos > 100) {
    scrollProgress.style.display = "flex";
  } else {
    scrollProgress.style.display = "none";
  }
  scrollProgress.addEventListener("click", () => {
    document.documentElement.scrollTop = 0;
  });
  scrollProgress.style.background = `conic-gradient(#000000 ${scrollValue}%, #f3f2f1 ${scrollValue}%)`;
};

window.onscroll = calcScrollValue;
window.onload = calcScrollValue;
// FOR SCROLL TO TOP


