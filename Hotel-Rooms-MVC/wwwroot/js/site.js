// // Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// // for details on configuring this project to bundle and minify static web assets.
//
// // Write your JavaScript code.
//
// // room.js - place this in your Scripts folder
// document.addEventListener('DOMContentLoaded', function() {
//     // Get DOM elements
//     const addRoomBtn = document.getElementById('addRoomBtn');
//     const roomModal = document.getElementById('roomModal');
//     const closeBtn = document.querySelector('.close');
//     const cancelBtn = document.getElementById('cancelBtn');
//     const roomForm = document.getElementById('roomForm');
//     const modalTitle = document.getElementById('modalTitle');
//
//     // Show modal when Add Room button is clicked
//     addRoomBtn.addEventListener('click', function() {
//         modalTitle.textContent = 'Add New Room';
//         roomForm.reset();
//         document.getElementById('roomId').value = '';
//         roomForm.action = '/Room/Create'; // Set the form action for create
//         roomModal.style.display = 'block';
//     });
//
//     // Close modal when X button is clicked
//     closeBtn.addEventListener('click', function() {
//         roomModal.style.display = 'none';
//     });
//
//     // Close modal when Cancel button is clicked
//     cancelBtn.addEventListener('click', function() {
//         roomModal.style.display = 'none';
//     });
//
//     // Close modal when clicking outside of it
//     window.addEventListener('click', function(event) {
//         if (event.target == roomModal) {
//             roomModal.style.display = 'none';
//         }
//     });
//
//     // Handle edit button clicks - we'll attach this to buttons dynamically
//     function setupEditButtons() {
//         const editButtons = document.querySelectorAll('.btn-edit');
//         editButtons.forEach(button => {
//             button.addEventListener('click', function() {
//                 const roomId = this.getAttribute('data-id');
//
//                 // Fetch room data via AJAX
//                 fetch(`/Room/GetRoom/${roomId}`)
//                     .then(response => response.json())
//                     .then(data => {
//                         // Populate the form with the room data
//                         document.getElementById('roomId').value = data.id;
//                         document.getElementById('roomName').value = data.name;
//                         document.getElementById('roomRate').value = data.rate;
//                         document.getElementById('roomSpace').value = data.spaceByMeter;
//                         document.getElementById('roomCapacity').value = data.capacity;
//                         document.getElementById('roomFacilities').value = data.facilities;
//
//                         // Update modal title and form action
//                         modalTitle.textContent = 'Edit Room';
//                         roomForm.action = '/Room/Edit';
//
//                         // Show the modal
//                         roomModal.style.display = 'block';
//                     })
//                     .catch(error => {
//                         console.error('Error fetching room data:', error);
//                         alert('Error fetching room data. Please try again.');
//                     });
//             });
//         });
//     }
//
//     // Handle delete button clicks
//     function setupDeleteButtons() {
//         const deleteButtons = document.querySelectorAll('.btn-danger');
//         deleteButtons.forEach(button => {
//             button.addEventListener('click', function() {
//                 const roomId = this.getAttribute('data-id');
//
//                 // Confirm deletion
//                 if (confirm('Are you sure you want to delete this room?')) {
//                     // Submit delete request via POST
//                     const form = document.createElement('form');
//                     form.method = 'POST';
//                     form.action = '/Room/Delete';
//
//                     const idInput = document.createElement('input');
//                     idInput.type = 'hidden';
//                     idInput.name = 'id';
//                     idInput.value = roomId;
//
//                     // Add CSRF token if your MVC app requires it
//                     const antiforgeryToken = document.querySelector('input[name="__RequestVerificationToken"]');
//                     if (antiforgeryToken) {
//                         const tokenInput = document.createElement('input');
//                         tokenInput.type = 'hidden';
//                         tokenInput.name = '__RequestVerificationToken';
//                         tokenInput.value = antiforgeryToken.value;
//                         form.appendChild(tokenInput);
//                     }
//
//                     form.appendChild(idInput);
//                     document.body.appendChild(form);
//                     form.submit();
//                 }
//             });
//         });
//     }
//
//     // Call these setup functions initially and whenever you refresh the room list
//     setupEditButtons();
//     setupDeleteButtons();
//
//     // Function to refresh room list via AJAX (optional, can be used after add/edit operations)
//     function refreshRoomList() {
//         fetch('/Room/GetAllRooms')
//             .then(response => response.text())
//             .then(html => {
//                 document.getElementById('roomList').innerHTML = html;
//                 setupEditButtons();
//                 setupDeleteButtons();
//             })
//             .catch(error => {
//                 console.error('Error refreshing room list:', error);
//             });
//     }
// });
