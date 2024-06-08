var dndJsInterop = {
    scrollToTop: function (scrollableAreaElementId, htmlElementId) {
        var scrollableArea = document.getElementById(scrollableAreaElementId);
        var specificElement = document.getElementById(htmlElementId);
        var arbitraryOffset = 7;
        var scrollPosition = specificElement.offsetTop - scrollableArea.offsetTop - arbitraryOffset;
        scrollableArea.scrollTo({ top: scrollPosition, behavior: "smooth" });
    },

    lockScroll: function (scrollableAreaElementId) {
        document.getElementById(scrollableAreaElementId).style.overflow = "hidden";
    },
    unlockScroll: function (scrollableAreaElementId) {
        document.getElementById(scrollableAreaElementId).style.overflow = "auto";
    },
}