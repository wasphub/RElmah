relmah = (function () {
    "use strict";

    return function (endpoint, subs) {
        var errors = new Rx.Subject(),
            applications = new Rx.Subject(),
            es, as,
            apps = {};

        subs = subs || {};

        return {
            start: function (opts) {
                var conn = $.hubConnection(endpoint),
                    proxy = conn.createHubProxy('relmah-errors');

                proxy.on('error', function (p) {
                    errors.onNext(p);
                });
                proxy.on('applications', function (existingApps, removedApps) {
                    var i, k;
                    for (i = 0; i < existingApps.length; i++) {
                        k = existingApps[i];
                        !apps[k] && applications.onNext({ name: k, removed: false });
                        apps[k] = true;
                    }
                    for (i = 0; removedApps && i < removedApps.length; i++) {
                        k = removedApps[i];
                        apps[k] && applications.onNext({ name: k, removed: true });
                        apps[k] = false;
                    }
                });

                conn.qs = { user: opts && opts.user };
                return conn.start().done(function () { });
            },
            getErrors: function () {
                es = es || (subs.errors && typeof (subs.errors) === 'function' && subs.errors(errors) || errors);
                return es;
            },
            getApplications: function () {
                as = as || (subs.applications && typeof (subs.applications) === 'function' && subs.applications(applications) || applications);
                return as;
            }
        };
    };
})();