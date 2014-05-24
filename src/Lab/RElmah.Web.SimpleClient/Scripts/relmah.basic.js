(function() {
    "use strict";

    if (!relmah) {
        throw new Error("You must reference relmah.js first");
    }

    relmah.basic = function(r) {
        return {
            getTotals: function() {
                return r.getErrors().scan(0, function(a, e) {
                    return a + 1;
                });
            }
        };
    };

})();