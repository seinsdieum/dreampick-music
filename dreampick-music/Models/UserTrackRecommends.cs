using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using dreampick_music.DbContexts;
using dreampick_music.DbRepositories;

namespace dreampick_music.Models;

public class UserTrackRecommends
{
    
    /*

GetRandomUserTracks(userId, int n) :
tracks = []
i = 0
while(i < n)
  t = repo.RandomTrackByUser(userId)
  if(!(t in tracks) tracks.Add(t)
  i++
return tracks

         */


    private static async Task<ICollection<Track>> GetRandomUserTracks(string userId, int n = 10)
    {

        var repo = new TrackRepository();
        
        var tracks = new List<Track>();
        for (int i = 0; i < n; i++)
        {
            try
            {
                var track = await repo.GetRandomByUserId(userId);
                if(tracks.All(t => t.Id != track.Id)) tracks.Add(track);
            } catch{}
        }

        return tracks;
    }

    
    /*


GetRandomUsersByTrackCollection(track[] ts) :
users = []
i = 0
while(i < ts.Count)
u = repo.RandomUserByTrack(trackId)
if !(u in users) users.Add(u)
i++;
return users


 */


    private static async Task<ICollection<User>> GetRandomUsersByTracks(ICollection<Track> tracks)
    {
        var repo = new UserRepository();
        
        var users = new List<User>();
        foreach (var track in tracks)
        {
            try
            {
                var user = await repo.GetRandomByTrackId(track.Id);
                if(users.All(u => u.Id != user.Id)) users.Add(user);
            }
            catch {}
        }

        return users;
    }


    private static async Task<ICollection<Track>> GetRecommendedTracks(ICollection<User> users, int n = 3)
    {
        var repo = new TrackRepository();
        var tracks = new List<Track>();

        foreach (var user in users)
        {
            for (var i = 0; i < n; i++)
            {
                try
                {
                    var track = await repo.GetRandomByUserId(user.Id);
                    if(tracks.All(t => t.Id != track.Id)) tracks.Add(track);
                } catch {}
            }
        }

        return tracks;
    }
    /*
     *
     *
     * GetRecommendations(string userId) :
         * ts = GetRandomUserTracks(userId)
         * users = GetRandomUsersByTracks(ts)
         * tracks = GetRandomUsersTracks(users, 5)
     * 
     */
    

    public static async Task<ICollection<Track>> GetRecommendedTrackCollection(string userId, int randomTrackIterations, int randomRecommendIterations)
    {
        var prepared = await GetRandomUserTracks(userId, randomTrackIterations);
        var users = await GetRandomUsersByTracks(prepared);
        var tracks = await GetRecommendedTracks(users, randomRecommendIterations);

        return tracks;
    }
}