using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ncfe.CodeTest
{
    public class LearnerService
    {
        private List<FailoverEntry> _failoverEntries;
        private LearnerResponse _learnerResponse;
        private static int _learnerId;
        private int _failedRequests;

        public Learner GetLearner(int learnerId, bool isLearnerArchived)
        {
            _learnerId = learnerId;

            if (isLearnerArchived)
                return GetArchivedLearner();

            else
                return GetLearner();
        }

        /// <summary>
        /// Retrieves a learner instance based on the current learner response.
        /// </summary>
        /// <returns>A Learner object representing the retrieved learner.</returns>
        private Learner GetLearner()
        {
            GetFailoverEntries();
            CountFailedRequests();
            GetLearnerResponse();

            return GetLearnerFromLearnerResponse();
        }

        /// <summary>
        /// Retrieves the learner from the learner response, returning the archived learner if the response is archived.
        /// </summary>
        /// <returns>A Learner instance from the learner response or the archived learner.</returns>
        private Learner GetLearnerFromLearnerResponse()
        {
            Learner learner;

            if (_learnerResponse.IsArchived)
                learner = GetArchivedLearner();

            else
                learner = _learnerResponse.Learner;

            return learner;
        }

        /// <summary>
        /// Retrieves learner response data from the appropriate data store based on the current failover mode.
        /// </summary>
        private void GetLearnerResponse()
        {
            _learnerResponse = null;

            if (IsFailoverMode())
                _learnerResponse = GetLearnersFromFailOverStore();

            else
                _learnerResponse = GetLearnersFromDataStore();
        }

        /// <summary>
        /// Retrieves learner information from the data store for the specified learner ID.
        /// </summary>
        /// <param name="learnerId">The unique identifier of the learner.</param>
        /// <returns>A LearnerResponse containing the learner's information.</returns>
        private static LearnerResponse GetLearnersFromDataStore()
        {
            LearnerResponse learnerResponse;
            var dataAccess = new LearnerDataAccess();
            learnerResponse = dataAccess.LoadLearner(_learnerId);

            return learnerResponse;
        }

        /// <summary>
        /// Retrieves learner information from the failover store.
        /// </summary>
        /// <returns>A LearnerResponse containing the learner's information.</returns>
        private static LearnerResponse GetLearnersFromFailOverStore()
        {
            return FailoverLearnerDataAccess.GetLearnerById(_learnerId);
        }

        /// <summary>
        /// Determines whether failover mode is enabled based on the number of failed requests and application settings.
        /// </summary>
        /// <returns>true if failover mode is enabled; otherwise, false.</returns>
        private bool IsFailoverMode()
        {
            return _failedRequests > 100 && 
                (ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "true" || 
                ConfigurationManager.AppSettings["IsFailoverModeEnabled"] == "True");
        }

        /// <summary>
        /// Check each failover entry that has failed within the last 10 minutes
        /// </summary>
        private void CountFailedRequests()
        {
            _failedRequests = 0;

            foreach (var failoverEntry in _failoverEntries)
            {
                if (IsFailoverEntry(failoverEntry))
                    _failedRequests++;
            }
        }

        /// <summary>
        /// Determines whether the failover entry represents a failed request within the last 10 minutes.
        /// </summary>
        /// <param name="failoverEntry">The failover entry to evaluate.</param>
        /// <returns>true if the failover entry occurred within the last 10 minutes; otherwise, false.</returns>
        private static bool IsFailoverEntry(FailoverEntry failoverEntry)
        {
            return failoverEntry.DateTime > DateTime.Now.AddMinutes(-10);
        }

        /// <summary>
        /// Retrive fail over entries
        /// </summary>
        private void GetFailoverEntries()
        {
            var failoverRespository = new FailoverRepository();
            _failoverEntries = failoverRespository.GetFailOverEntries();
        }

        /// <summary>
        /// Get learner data from archive
        /// </summary>
        /// <returns>Learner</returns>
        private static Learner GetArchivedLearner()
        {
            var archivedDataService = new ArchivedDataService();
            Learner archivedLearner = archivedDataService.GetArchivedLearner(_learnerId);

            return archivedLearner;
        }
    }
}
