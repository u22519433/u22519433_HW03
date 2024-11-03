$(document).ready(function () {
    // Initialize Bootstrap modals
    var studentModal = new bootstrap.Modal(document.getElementById('createStudentModal'));
    var bookModal = new bootstrap.Modal(document.getElementById('createBookModal'));

    // Handle student form submission
    $('.student-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);

        if (form.valid()) {
            $.ajax({
                url: form.attr('action'),
                method: 'POST',
                data: form.serialize(),
                success: function (response) {
                    studentModal.hide();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    alert('Error occurred while adding student.');
                }
            });
        }
    });

    // Handle book form submission
    $('.book-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);

        if (form.valid()) {
            $.ajax({
                url: form.attr('action'),
                method: 'POST',
                data: form.serialize(),
                success: function (response) {
                    bookModal.hide();
                    location.reload();
                },
                error: function (xhr, status, error) {
                    alert('Error occurred while adding book.');
                }
            });
        }
    });

    // Clear form when modal is hidden
    $('#createStudentModal, #createBookModal').on('hidden.bs.modal', function () {
        var form = $(this).find('form');
        form[0].reset();
        form.find('.text-danger').empty();
    });
});

