relmah = (function () {
    "use strict";

    function FilteredSubject(filter, previous) {
        var that = this;

        previous && previous.dispose && previous.dispose();

        that.subject  = new Rx.Subject();
        that.filtered = filter && typeof (filter) === 'function' && filter(that.subject) || that.subject;

        that.dispose = function () {
            that.subject.dispose();
            that.filtered.dispose();
        }

        that.onNext = function (n) {
            that.subject.onNext(n);
        }
    }

    return function (endpoint, subs) {
        var conn,
            errors,
            applications,
            apps = {},
            starting;

        subs = subs || {};

        return {
            start: function (opts) {
                conn = $.hubConnection(endpoint);

                var proxy = conn.createHubProxy('relmah-errors');

                errors       = new FilteredSubject(subs.errors, errors);
                applications = new FilteredSubject(subs.applications, applications);

                proxy.on('error', function (p) {
                    errors.onNext(p);
                });
                proxy.on('applications', function (existingApps, removedApps) {
                    var i, k;
                    for (i = 0; i < existingApps.length; i++) {
                        k = existingApps[i];
                        !apps[k] && applications.onNext({ name: k, action: 'added' });
                        apps[k] = true;
                    }
                    for (i = 0; removedApps && i < removedApps.length; i++) {
                        k = removedApps[i];
                        apps[k] && applications.onNext({ name: k, action: 'removed' });
                        apps[k] = false;
                    }
                });

                starting && starting();

                conn.qs = { user: opts && opts.user };
                return conn.start();
            },
            getErrors: function () {
                return errors.filtered;
            },
            getApplications: function () {
                return applications.filtered;
            },
            stop: function () {
                apps = {};
                return conn && conn.stop();
            },
            starting: function(callback) {
                starting = callback;
            },
            disconnected: function(callback) {
                return conn.disconnected(callback);
            }
        };
    };
})();