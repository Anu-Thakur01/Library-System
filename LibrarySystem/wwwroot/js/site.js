window.libraryDataTable = function (selector) {
    if (!window.jQuery || !$.fn.DataTable) {
        return;
    }

    $(selector).DataTable({
        pageLength: 10,
        lengthMenu: [10, 25, 50, 100],
        language: {
            search: "",
            searchPlaceholder: "Search records...",
            lengthMenu: "Rows per page _MENU_",
            info: "Showing _START_ to _END_ of _TOTAL_ records",
            infoEmpty: "No records available",
            infoFiltered: "(filtered from _MAX_ total)",
            zeroRecords: "No matching records found",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        }
    });
};

// Navigate back with graceful fallback to the dashboard
window.goBack = function (fallbackUrl) {
    try {
        if (window.history && window.history.length > 1) {
            window.history.back();
            return;
        }
    }
    catch (e) {
        // ignore and fallback
    }

    if (fallbackUrl) {
        window.location.href = fallbackUrl;
    }
    else {
        window.location.href = '/';
    }
};
