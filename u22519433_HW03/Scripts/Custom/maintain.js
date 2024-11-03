$(document).ready(function () {
    // Initialize all modals
    var modals = {
        createType: new bootstrap.Modal(document.getElementById('createTypeModal')),
        createAuthor: new bootstrap.Modal(document.getElementById('createAuthorModal')),
        createBorrow: new bootstrap.Modal(document.getElementById('createBorrowModal')),
        editType: new bootstrap.Modal(document.getElementById('editTypeModal')),
        editAuthor: new bootstrap.Modal(document.getElementById('editAuthorModal'))
    };

    // Get the anti-forgery token
    var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

    // Handle form submissions
    $('.modal form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);

        if (form.valid()) {
            $.ajax({
                url: form.attr('action'),
                method: 'POST',
                data: form.serialize(),
                success: function () {
                    location.reload();
                },
                error: function () {
                    alert('An error occurred while processing your request.');
                }
            });
        }
    });

    // Handle Type edit button clicks
    $('.edit-type').click(function () {
        var id = $(this).data('id');
        var name = $(this).data('name');

        var form = $('#editTypeModal form');
        form.find('[name="TypeId"]').val(id);
        form.find('[name="Name"]').val(name);

        modals.editType.show();
    });

    // Handle Author edit button clicks
    $('.edit-author').click(function () {
        var id = $(this).data('id');
        var name = $(this).data('name');
        var surname = $(this).data('surname');

        var form = $('#editAuthorModal form');
        form.find('[name="AuthorId"]').val(id);
        form.find('[name="Name"]').val(name);
        form.find('[name="Surname"]').val(surname);

        modals.editAuthor.show();
    });

    // Handle return book button clicks
    $('.return-book').click(function () {
        if (confirm('Are you sure you want to mark this book as returned?')) {
            var borrowId = $(this).data('id');

            $.ajax({
                url: '/Maintain/UpdateBorrow',
                method: 'POST',
                data: {
                    borrowId: borrowId,
                    broughtDate: new Date().toISOString(),
                    __RequestVerificationToken: antiForgeryToken
                },
                success: function () {
                    location.reload();
                },
                error: function () {
                    alert('Error marking book as returned.');
                }
            });
        }
    });

    // Handle delete buttons
    $('.delete-type, .delete-author, .delete-borrow').click(function () {
        var button = $(this);
        var type = button.hasClass('delete-type') ? 'type' :
            button.hasClass('delete-author') ? 'author' : 'borrow';
        var id = button.data('id');

        if (confirm(`Are you sure you want to delete this ${type}?`)) {
            $.ajax({
                url: `/Maintain/Delete${type.charAt(0).toUpperCase() + type.slice(1)}/${id}`,
                method: 'POST',
                headers: {
                    'RequestVerificationToken': antiForgeryToken
                },
                data: { __RequestVerificationToken: antiForgeryToken },
                success: function (response) {
                    if (response.success) {
                        location.reload();
                    } else {
                        console.log('Delete failed:', response.message);
                        alert(`Error deleting ${type}: ${response.message}`);
                    }
                },
                error: function (xhr, status, error) {
                    console.log('Ajax error:', error);
                    console.log('Status:', status);
                    console.log('Response:', xhr.responseText);
                    alert(`Error deleting ${type}. See console for details.`);
                }
            });
        }
    });

    // Clear forms when modals are hidden
    $('.modal').on('hidden.bs.modal', function () {
        var form = $(this).find('form');
        form[0].reset();
        form.find('.text-danger').empty();
    });
});

