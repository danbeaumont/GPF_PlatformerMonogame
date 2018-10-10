using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PlatformerMonogame1
{
    class Collision
    {
        public Game1 game;

        public bool IsColliding(Sprite hero, Sprite otherSprite)
        {
            // Compare the position of each rectangle edge against the other
            // It compares opposite edges of each rectangle, ie, the left edge of
            // the right of the other
            if (hero.rightEdge < otherSprite.leftEdge ||
                hero.leftEdge > otherSprite.rightEdge ||
                hero.bottomEdge < otherSprite.topEdge ||
                hero.topEdge > otherSprite.bottomEdge)
            {
                // these rectangles are NOT colliding
                return false;
            }

            // else, the two AABB rectangels are overlapping and there is a collision
            return true;
        }

        public Sprite CollideWithPlatforms(Sprite hero, float deltaTime)
        {
            // Create a copy of the hero that will move to where the hero will be 
            // on the next frame, to PREDICT if the hero will overlap an obstacle
            Sprite playerPrediction = new Sprite();
            playerPrediction.position = hero.position;
            playerPrediction.width = hero.width;
            playerPrediction.height = hero.height;
            playerPrediction.offset = hero.offset;
            playerPrediction.UpdateHitBox();

            playerPrediction.position += hero.velocity * deltaTime;

            // remove Math.Round
            int playerColumn = (int)playerPrediction.position.X / game.tileHeight;
            int playerRow = (int)playerPrediction.position.Y / game.tileHeight;
            Vector2 playerTile = new Vector2(playerColumn, playerRow);

            Vector2 leftTile = new Vector2(playerTile.X - 1, playerTile.Y);
            Vector2 rightTile = new Vector2(playerTile.X + 1, playerTile.Y);
            Vector2 topTile = new Vector2(playerTile.X, playerTile.Y - 1);
            Vector2 bottomTile = new Vector2(playerTile.X, playerTile.Y + 1);

            Vector2 bottomLeftTile = new Vector2(playerTile.X - 1, playerTile.Y + 1);
            Vector2 bottomRightTile = new Vector2(playerTile.X + 1, playerTile.Y + 1);
            Vector2 topLeftTile = new Vector2(playerTile.X - 1, playerTile.Y - 1);
            Vector2 topRightTile = new Vector2(playerTile.X + 1, playerTile.Y - 1);

            bool leftCheck = CheckForTile(leftTile);
            bool rightCheck = CheckForTile(rightTile);
            bool bottomCheck = CheckForTile(bottomTile);
            bool topCheck = CheckForTile(topTile);

            bool bottomLeftCheck = CheckForTile(bottomLeftTile);
            bool bottomRightCheck = CheckForTile(bottomRightTile);
            bool topLeftCheck = CheckForTile(topLeftTile);
            bool topRightCheck = CheckForTile(topRightTile);

            // checks for collisions with tiles to the left of the player
            if (leftCheck == true)
            {
                hero = CollideLeft(hero, leftTile, playerPrediction);
            }
            // checks for collisions with tiles to the right of the player
            if (rightCheck == true)
            {
                hero = CollideRight(hero, rightTile, playerPrediction);
            }
            // check for collisions with tiles under the player
            if (bottomCheck == true)
            {
                hero = CollideBelow(hero, bottomTile, playerPrediction);
            }
            // check for collisions with tiles above the player
            if (topCheck == true)
            {
                hero = CollideAbove(hero, topTile, playerPrediction);
            }
            // check for collision with tiles below and to the left of the player
            if (leftCheck == false && bottomCheck == false && bottomLeftCheck == true)
            {
                //properly check for diagonal collisions
                hero = CollideBottomDiagonals(hero, bottomLeftTile, playerPrediction);
            }
            // check for collision with tiles below and to the right of the player
            if (rightCheck == false && bottomCheck == false && bottomRightCheck == true)
            {
                // properly check for diagonal collisions
                hero = CollideBottomDiagonals(hero, bottomRightTile, playerPrediction);
            }
            // check for collision with tiles above and to the left of the player
            if (leftCheck == false && topCheck == false && topLeftCheck == true)
            {
                // proper check for diagonal collisions
                hero = CollideAboveDiagonals(hero, topLeftTile, playerPrediction);
            }
            // check for collision with tiles above and to the right of the player
            if (rightCheck == false && topCheck == false && topRightCheck == true)
            {
                // proper check for diag collisions
                hero = CollideAboveDiagonals(hero, topRightTile, playerPrediction);
            }

            return hero;
        }

        // Check if there is a tile at the specified coordinate
        bool CheckForTile(Vector2 coordinates)
        {
            int column = (int)coordinates.X;
            int row = (int)coordinates.Y;

            if (column < 0 || column > game.levelTileWidth - 1)
            {
                return false;
            }
            if (row < 0 || row > game.levelTileHeight - 1)
            {
                return false;
            }

            Sprite tileFound = game.levelGrid[column, row];

            if (tileFound != null)
            {
                return true;
            }

            return false;
        }

        Sprite CollideLeft(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            // remove velocity check
            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X < 0)
            {
                hero.position.X = tile.rightEdge + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero;
        }

        Sprite CollideRight(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            // remove velocity check
            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X > 0)
            {
                // remove hero widtch
                hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero;
        }

        Sprite CollideAbove(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            // remove velocity check
            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y < 0)
            {
                hero.position.Y = tile.bottomEdge + hero.offset.Y;
                hero.velocity.Y = 0;
            }

            return hero;
        }

        Sprite CollideBelow(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            // remove velocity check
            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y > 0)
            {
                // remove hero height
                hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                hero.velocity.Y = 0;
            }

            return hero;
        }

        Sprite CollideBottomDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            int leftEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int rightEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int topEdgeDistance = Math.Abs(tile.topEdge - playerPrediction.bottomEdge);

            if (IsColliding(playerPrediction,tile) == true)
            {
                if (topEdgeDistance < rightEdgeDistance && topEdgeDistance < leftEdgeDistance)
                {
                    // if top edge is closest, collision is happening above the platform
                    //********** remove hero height
                    hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                    hero.velocity.Y = 0;
                }
                else if (rightEdgeDistance < leftEdgeDistance)
                {
                    // if right edge is closest, collision is happening to the right of the platform
                    hero.position.X = tile.rightEdge + hero.offset.X;
                }
                else
                {
                    // else if left edge is closest, collision is happening to the left of the platform
                    // ***********remove hero width
                    hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                }
            }
            return hero;
        }

        Sprite CollideAboveDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            int leftEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int rightEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int bottomEdgeDistance = Math.Abs(tile.bottomEdge - playerPrediction.topEdge);

            if(IsColliding(playerPrediction, tile) == true)
            {
                if(bottomEdgeDistance < leftEdgeDistance && bottomEdgeDistance < rightEdgeDistance)
                {
                    // if the bottom edge is closes and overlapping on the top edge
                    hero.position.Y = tile.bottomEdge + hero.offset.Y;
                }
                else if (leftEdgeDistance < rightEdgeDistance)
                {
                    // else if left edge is closest and overlapping
                    hero.position.X = tile.rightEdge + hero.offset.X;
                }
                else
                {
                    // else if right edge is closest and overlapping
                    // *******remove hero width
                    hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                }
            }
            return hero;
        }

    }
}
