(function(root) {
    "use strict";

    if (!root) {
        throw new Error("You must reference relmah.js first");
    }

    root.basic = function (r) {
        return {
            getTotals: function() {
                return r.getErrors().scan(0, function(a, e) {
                    return a + 1;
                });
            }
        };
    };

})(relmah);